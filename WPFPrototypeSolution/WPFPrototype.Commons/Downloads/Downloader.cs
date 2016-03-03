using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 文件下载器
    /// </summary>
    public class Downloader
    {
        #region fields
        /// <summary>
        /// 最小分片大小
        /// </summary>
        public const long SegmentMinSize = 1024 * 512;
        /// <summary>
        /// 默认线程数
        /// </summary>
        public const int DefaultThreadCount = 4;
        /// <summary>
        /// 空缓存的长度
        /// </summary>
        public const int EmptyCacheLength = 0;
        /// <summary>
        /// 空的WorkID
        /// </summary>
        public const long EmptyWorkID = 0;
        /// <summary>
        /// 下载进度计时器间隔，毫秒
        /// </summary>
        public const long ProgressTimerInterval = 500;
        /// <summary>
        /// 用于lock操作
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// 下载线程数，代表最大可以同时下载的线程数
        /// </summary>
        private int _maxThreadCount;
        /// <summary>
        /// 分片策略
        /// </summary>
        private ISegmentCalculator _segmentCalculator;
        /// <summary>
        /// 输入源管理
        /// </summary>
        private SourceProvider _sourceProvider;
        /// <summary>
        /// 负责写入下载数据到本地
        /// </summary>
        private LocalFileWriter _writer;
        /// <summary>
        /// 文件片段下载线程
        /// </summary>
        private List<SegmentThread> _threads;
        /// <summary>
        /// 缓存上次没有下载完成的LocalFileConfig
        /// </summary>
        private LocalFileConfig _config;
        /// <summary>
        /// 文件保存路径
        /// </summary>
        private string _savePath;
        /// <summary>
        /// 临时文件保存路径
        /// </summary>
        private string _tmpPath;
        /// <summary>
        /// 配置文件保存路径
        /// </summary>
        private string _configPath;
        /// <summary>
        /// 缓存长度
        /// </summary>
        private int _cacheLength;
        /// <summary>
        /// 工作ID，用来在多线程中检测当前操作是否已经过期
        /// </summary>
        private long _workID;
        /// <summary>
        /// 获取下载进度的Timer
        /// </summary>
        private Timer _progressTimer;
        /// <summary>
        /// 最后一次下载进度改变时间戳
        /// </summary>
        private DateTime _lastProgressChangeTime;
        /// <summary>
        /// 最后一次下载进度改变时已经下载的字节
        /// </summary>
        private long _lastDownloadedSize;
        #endregion

        #region constructors
        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="configPath">本地文件</param>
        /// <param name="cacheLength">缓存大小，缓存可减少硬盘写入压力，不使用缓存可设置为<see cref="Downloader.EmptyCacheLength"/></param>
        public Downloader(string configPath, int cacheLength)
            : this(configPath, DefaultThreadCount, cacheLength)
        {
        }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="configPath">本地文件</param>
        /// <param name="threadCount">下载线程数</param>
        /// <param name="cacheLength">缓存大小，缓存可减少硬盘写入压力，不使用缓存可设置为<see cref="Downloader.EmptyCacheLength"/></param>
        public Downloader(string configPath, int threadCount, int cacheLength)
        {
            if (string.IsNullOrEmpty(configPath)) throw new ArgumentException("configPath can not be null or empty.");

            var localFileConfig = LocalFileConfig.Load(configPath);

            if (localFileConfig.MainSource == null) throw new Exception("LocalFileConfig.MainSource can not be null.");
            if (localFileConfig.RemoteInfo == null) throw new Exception("LocalFileConfig.RemoteInfo can not be null.");
            if (localFileConfig.RemoteInfo.Size < 0) throw new Exception("LocalFileConfig.RemoteInfo.Size must bigger than 0.");
            if (!localFileConfig.HasSegment) throw new Exception("LocalFileConfig.Segments can not be empty.");

            this._workID = EmptyWorkID;
            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);
            this._config = localFileConfig;
            this._maxThreadCount = threadCount;
            this._configPath = configPath;
            this._savePath = DownloadHelper.GetFilePathFromConfigPath(configPath);
            this._tmpPath = DownloadHelper.GetTmpPathFromFilePath(this._savePath);
            this._cacheLength = cacheLength;
            this._status = DownloaderStatuses.Idle;
        }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="cacheLength">缓存大小，缓存可减少硬盘写入压力，不使用缓存可设置为<see cref="Downloader.EmptyCacheLength"/></param>
        public Downloader(string url, string savePath, int cacheLength)
           : this(url, null, savePath, DefaultThreadCount, cacheLength)
        { }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="threadCount">下载线程数</param>
        /// <param name="cacheLength">缓存大小，缓存可减少硬盘写入压力，不使用缓存可设置为<see cref="Downloader.EmptyCacheLength"/></param>
        public Downloader(string url, string savePath, int threadCount, int cacheLength)
           : this(url, null, savePath, threadCount, cacheLength)
        { }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="mirrors">镜像地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="cacheLength">缓存大小，缓存可减少硬盘写入压力，不使用缓存可设置为<see cref="Downloader.EmptyCacheLength"/></param>
        public Downloader(string url, IEnumerable<string> mirrors, string savePath, int cacheLength)
            : this(url, mirrors, savePath, DefaultThreadCount, cacheLength)
        { }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="mirrors">镜像地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="threadCount">下载线程数</param>
        /// <param name="cacheLength">缓存大小，缓存可减少硬盘写入压力，不使用缓存可设置为<see cref="Downloader.EmptyCacheLength"/></param>
        public Downloader(string url, IEnumerable<string> mirrors, string savePath, int threadCount, int cacheLength)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("url can not be null or empty.");
            if (string.IsNullOrEmpty(savePath)) throw new ArgumentException("savePath can not be null or empty.");
            if (threadCount <= 0) throw new ArgumentException("threadCount must bigger than 0.");

            this._workID = EmptyWorkID;
            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);
            // 初始化_localFileCache
            var mainSource = new FileSource(url);
            List<FileSource> mirrorSources = null;
            if (mirrors != null)
            {
                mirrorSources = new List<FileSource>();
                foreach (var mirror in mirrors)
                {
                    mirrorSources.Add(new FileSource(mirror));
                }
            }// if

            this._config = new LocalFileConfig
            {
                MainSource = mainSource,
                Mirrors = mirrorSources,
            };

            this._maxThreadCount = threadCount;
            this._savePath = savePath;
            this._configPath = DownloadHelper.GetConfigPathFromFilePath(savePath);
            this._tmpPath = DownloadHelper.GetTmpPathFromFilePath(savePath);
            this._cacheLength = cacheLength;
            this._status = DownloaderStatuses.Idle;
        }
        #endregion

        #region properties
        #region Status
        private DownloaderStatuses _status;
        /// <summary>
        /// Get or set <see cref="Status"/>
        /// </summary>
        public DownloaderStatuses Status
        {
            get { return this._status; }
        }
        #endregion
        #endregion

        #region events
        #region Completed
        /// <summary>
        /// Event name of <see cref="Completed"/>
        /// </summary>
        public const string CompletedEventName = "Completed";

        public event EventHandler<EventArgs> Completed;

        private void OnCompleted()
        {
            EventHandler<EventArgs> handler = this.Completed;

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakCompletedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.AddHandler(this, CompletedEventName, handler);
        }

        public void RemoveWeakCompletedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.RemoveHandler(this, CompletedEventName, handler);
        }
        #endregion

        #region Failed
        /// <summary>
        /// Event name of <see cref="Failed"/>
        /// </summary>
        public const string FailedEventName = "Failed";

        public event EventHandler<EventArgs> Failed;

        private void OnFailed()
        {
            EventHandler<EventArgs> handler = this.Failed;

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakFailedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.AddHandler(this, FailedEventName, handler);
        }

        public void RemoveWeakFailedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.RemoveHandler(this, FailedEventName, handler);
        }
        #endregion

        #region Cancelled
        /// <summary>
        /// Event name of <see cref="Cancelled"/>
        /// </summary>
        public const string CancelledEventName = "Cancelled";

        public event EventHandler<EventArgs> Cancelled;

        private void OnCancelled()
        {
            EventHandler<EventArgs> handler = this.Cancelled;

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakCancelledHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.AddHandler(this, CancelledEventName, handler);
        }

        public void RemoveWeakCancelledHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.RemoveHandler(this, CancelledEventName, handler);
        }
        #endregion

        #region Paused
        /// <summary>
        /// Event name of <see cref="Paused"/>
        /// </summary>
        public const string PausedEventName = "Paused";

        public event EventHandler<EventArgs> Paused;

        private void OnPaused()
        {
            EventHandler<EventArgs> handler = this.Paused;

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakPausedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.AddHandler(this, PausedEventName, handler);
        }

        public void RemoveWeakPausedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.RemoveHandler(this, PausedEventName, handler);
        }
        #endregion

        #region Started
        /// <summary>
        /// Event name of <see cref="Started"/>
        /// </summary>
        public const string StartedEventName = "Started";

        public event EventHandler<EventArgs> Started;

        private void OnStarted()
        {
            EventHandler<EventArgs> handler = this.Started;

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakStartedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.AddHandler(this, StartedEventName, handler);
        }

        public void RemoveWeakStartedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<Downloader, EventArgs>.RemoveHandler(this, StartedEventName, handler);
        }
        #endregion

        #region ProgressChanged
        /// <summary>
        /// Event name of <see cref="ProgressChanged"/>
        /// </summary>
        public const string ProgressChangedEventName = "ProgressChanged";

        public event EventHandler<DownloadProgressArgs> ProgressChanged;

        private void OnProgressChanged(DownloadProgressArgs e)
        {
            EventHandler<DownloadProgressArgs> handler = this.ProgressChanged;

            if (handler != null) handler(this, e);
        }

        public void AddWeakProgressChangedHandler(EventHandler<DownloadProgressArgs> handler)
        {
            WeakEventManager<Downloader, DownloadProgressArgs>.AddHandler(this, ProgressChangedEventName, handler);
        }

        public void RemoveWeakProgressChangedHandler(EventHandler<DownloadProgressArgs> handler)
        {
            WeakEventManager<Downloader, DownloadProgressArgs>.RemoveHandler(this, ProgressChangedEventName, handler);
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 开始下载
        /// </summary>
        public async void Start()
        {
            long workID;
            SourceProvider sourceProvider;
            lock (this._syncRoot)
            {
                if (this._status == DownloaderStatuses.Downloading) throw new Exception("Downloader is working.");
                if (this._status == DownloaderStatuses.Completed) throw new Exception("Downloader is completed.");

                this._status = DownloaderStatuses.Downloading; // 设置Downloader状态

                // 生成新的workID
                workID = this.RefreshWorkID();

                // 重置SourceProvider
                var protocalProvider = new HttpProtocalProvider();
                var mirrorSelector = new SequentialMirrorSelector();
                mirrorSelector.Initialize(this._config.MainSource, this._config.Mirrors);
                sourceProvider = new SourceProvider(mirrorSelector, protocalProvider);
                this._sourceProvider = sourceProvider;
            }// lock

            // 初始化SourceProvider
            await sourceProvider.InitializeAsync(CancellationToken.None);

            lock (this._syncRoot)
            {
                // 检测当前操作是否过期
                if (workID != this._workID) return;

                var newRemoteFileInfo = this._sourceProvider.GetRemoteFileInfo();

                // 对文件进行分片
                List<LocalSegment> segments = new List<LocalSegment>();
                if (this._config.HasSegment &&
                    this._config.RemoteInfo != null &&
                    RemoteFileInfo.IsSameFile(this._config.RemoteInfo, newRemoteFileInfo))
                {
                    // 如果文件已经分片,服务器文件信息与缓存的文件信息一致则用缓存的文件分片
                    segments.AddRange(this._config.Segments);
                }

                // 如果没有分片或则缓存的文件已经过时了则重新分片
                if (segments.Count == 0)
                {
                    if (newRemoteFileInfo.IsAcceptRange)// 如果文件支持分段下载
                    {
                        var calculateSegments = this._segmentCalculator.GetSegments(this._maxThreadCount, newRemoteFileInfo);
                        foreach (var calculateSegment in calculateSegments)
                        {
                            segments.Add(new LocalSegment
                            {
                                StartPosition = calculateSegment.StartPosition,
                                EndPosition = calculateSegment.EndPosition
                            });
                        }
                    }
                    else// 如果文件不支持分段下载
                    {
                        if (newRemoteFileInfo.Size <= 0) throw new Exception(string.Format("Remote file size invalid, size: {0}.", newRemoteFileInfo.Size));
                        // 整个文件只有一个片段
                        segments.Add(new LocalSegment
                        {
                            StartPosition = 0,
                            EndPosition = newRemoteFileInfo.Size - 1
                        });
                        // 设置最大线程数为1
                        this._maxThreadCount = 1;
                    }// else
                }// if

                // 创建LocalFileWriter
                this._writer = new LocalFileWriter(this._tmpPath, segments, this._cacheLength);
                this._writer.CreateFile();

                // 缓存文件配置
                this.CacheConfig();

                // 创建下载线程
                this._threads = new List<SegmentThread>();
                for (int i = 0; i < this._maxThreadCount; i++)
                {
                    var thread = new SegmentThread(i + 1, this._writer, this._sourceProvider, workID);
                    thread.Completed += Thread_Completed;
                    thread.Failed += Thread_Failed;
                    this._threads.Add(thread);
                }

                // 启动下载线程
                foreach (var thread in this._threads)
                {
                    thread.Start();
                }

                this._lastProgressChangeTime = DateTime.Now;
                this._lastDownloadedSize = this._writer.GetDownloadedSize();
                this._progressTimer = new Timer(ProgressTimer_Callback, workID, ProgressTimerInterval, Timeout.Infinite);
            }// lock

            this.OnStarted();
        }

        /// <summary>
        /// 暂停，可以调用Start继续下载
        /// </summary>
        public void Pause()
        {
            lock (this._syncRoot)
            {
                if (this._status != DownloaderStatuses.Downloading) return;

                this._status = DownloaderStatuses.Paused;
                // 刷新WorkID
                this.RefreshWorkID();
                this._progressTimer.Dispose();
                this._progressTimer = null;
                this.ClearThread();
                // 缓存配置文件
                this.CacheConfig();
                // 清空SourceProvider
                this._sourceProvider = null;
                // 释放Writer
                this._writer.Dispose();
                this._writer = null;
            }// lock

            this.OnPaused();
        }

        /// <summary>
        /// 取消，会删除已经下载的文件
        /// </summary>
        public void Cancel()
        {
            lock (this._syncRoot)
            {
                if (this._status == DownloaderStatuses.Completed) return; // 如果已经下载完成则Cancel无效
                if (this._status == DownloaderStatuses.Cancelled) return; // 如果已经取消了，则不能重复操作

                if (this._status == DownloaderStatuses.Downloading)
                {
                    // 刷新WorkID
                    this.RefreshWorkID();
                    this._progressTimer.Dispose();
                    this._progressTimer = null;
                    this.ClearThread();
                    // 清空SourceProvider
                    this._sourceProvider = null;
                    // 释放Writer
                    this._writer.Dispose();
                    this._writer = null;
                }// if

                this._status = DownloaderStatuses.Cancelled;
                // 删除临时文件
                if (File.Exists(this._tmpPath)) File.Delete(this._tmpPath);
                // 删除配置文件
                if (File.Exists(this._configPath)) File.Delete(this._configPath);
            }// lock

            this.OnCancelled();
        }
        #endregion

        #region private methdos
        private void ProgressTimer_Callback(object state)
        {
            long workID = (long)state;
            DownloadProgressArgs args;
            // 获取下载状态
            lock (this._syncRoot)
            {
                if (workID != this._workID) return;// 这次下载已经过期

                var segments = this._writer.GetSegments();
                args = new DownloadProgressArgs(segments);
                var now = DateTime.Now;
                args.Speed = (args.DownloadedSize - this._lastDownloadedSize) / Math.Max(1, (long)(now - this._lastProgressChangeTime).TotalSeconds);
                this._lastProgressChangeTime = now;
                this._lastDownloadedSize = args.DownloadedSize;
            }// lock

            // 触发进度改变事件
            this.OnProgressChanged(args);

            // 启动下一次进度计时
            lock (this._syncRoot)
            {
                if (workID != this._workID) return;// 这次下载已经过期

                this._progressTimer.Change(ProgressTimerInterval, Timeout.Infinite);
            }
        }

        private void Thread_Failed(object sender, ThreadExitedArgs e)
        {
            bool isRaiseFailed = false;
            lock (this._syncRoot)
            {
                if (e.WorkID != this._workID) return;// 这次操作已经过期
                if (this._status != DownloaderStatuses.Downloading) return;// 只有Downloading状态才可能变为Failed状态

                this.RefreshWorkID();
                this._status = DownloaderStatuses.Failed;
                this._progressTimer.Dispose();
                this._progressTimer = null;
                this.ClearThread();
                this.CacheConfig();// 保存文件配置以便以后继续下载
                this._sourceProvider = null;
                this._writer.Dispose();
                this._writer = null;

                isRaiseFailed = true;
            }// lock

            if (isRaiseFailed) this.OnFailed();
        }

        private void Thread_Completed(object sender, ThreadExitedArgs e)
        {
            bool isRaiseCompleted = false;
            lock (this._syncRoot)
            {
                if (e.WorkID != this._workID) return;// 这次操作已经过期
                if (this._status != DownloaderStatuses.Downloading) return;// 只有Downloading状态才可能变为Failed状态
                if (!this._writer.IsCompleted()) return;// 文件下载还没有完成

                this.RefreshWorkID();
                this._status = DownloaderStatuses.Completed;
                this._progressTimer.Dispose();
                this._progressTimer = null;
                this.ClearThread();
                this._sourceProvider = null;
                this._writer.Dispose();
                this._writer = null;

                if (File.Exists(this._savePath)) File.Delete(this._savePath);
                File.Move(this._tmpPath, this._savePath);
                if (File.Exists(this._configPath)) File.Delete(this._configPath);

                isRaiseCompleted = true;
            }// lock

            if (isRaiseCompleted) this.OnCompleted();
        }

        private void ClearThread()
        {
            foreach (var thread in this._threads)
            {
                thread.Pause();
                thread.Completed -= Thread_Completed;
                thread.Failed -= Thread_Failed;
            }

            this._threads.Clear();
        }

        private void CacheConfig()
        {
            this._config.RemoteInfo = this._sourceProvider.GetRemoteFileInfo();
            this._config.Segments = this._writer.GetSegments();
            this._config.Save(this._configPath);
        }

        private long RefreshWorkID()
        {
            if (this._workID == long.MaxValue)
            {
                this._workID = EmptyWorkID + 1;
            }
            else
            {
                this._workID++;
            }

            return this._workID;
        }
        #endregion
    }
}
