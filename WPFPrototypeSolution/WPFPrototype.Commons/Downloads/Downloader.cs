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
        /// 缓存上次没有下载完成的LocalFileInfo
        /// </summary>
        private LocalFileInfo _localFileCache;
        #endregion

        #region constructors
        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="localFile">本地文件</param>
        public Downloader(LocalFileInfo localFile)
            : this(localFile, DefaultThreadCount)
        {
        }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="threadCount">下载线程数</param>
        public Downloader(LocalFileInfo localFile, int threadCount)
        {
            if (localFile.MainSource == null) throw new Exception("localFile.MainSource can not be null.");
            if (string.IsNullOrEmpty(localFile.SavePath)) throw new Exception("localFile.SavePath can not be null or empty.");
            if (localFile.RemoteInfo == null) throw new Exception("localFile.RemoteInfo can not be null.");
            if (localFile.RemoteInfo.Size < 0) throw new Exception("localFile.RemoteInfo.Size must bigger than 0.");
            if (!localFile.HasSegment) throw new Exception("localFile.Segments can not be empty.");

            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);

            // 初始化SourceProvider移动到Start方法里
            //var protocalProvider = new HttpProtocalProvider();
            //var mirrorSelector = new SequentialMirrorSelector();
            //mirrorSelector.Initialize(localFile.Source, localFile.Mirrors);
            //this._sourceProvider = new SourceProvider(mirrorSelector, protocalProvider);

            //this._writer = new LocalFileWriter(localFile.SavePath, localFile.RemoteInfo.Size, localFile.Segments); // writer要在Start的时候初始化
            this._localFileCache = localFile;
            this._maxThreadCount = threadCount;
        }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">保存路径</param>
        public Downloader(string url, string savePath)
           : this(url, null, savePath, DefaultThreadCount)
        { }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="threadCount">下载线程数</param>
        public Downloader(string url, string savePath, int threadCount)
           : this(url, null, savePath, threadCount)
        { }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="mirrors">镜像地址</param>
        /// <param name="savePath">保存路径</param>
        public Downloader(string url, IEnumerable<string> mirrors, string savePath)
            : this(url, mirrors, savePath, DefaultThreadCount)
        { }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="mirrors">镜像地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="threadCount">下载线程数</param>
        public Downloader(string url, IEnumerable<string> mirrors, string savePath, int threadCount)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("url can not be null or empty.");
            if (string.IsNullOrEmpty(savePath)) throw new ArgumentException("savePath can not be null or empty.");
            if (threadCount <= 0) throw new ArgumentException("threadCount must bigger than 0.");

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
            this._localFileCache = new LocalFileInfo
            {
                MainSource = mainSource,
                Mirrors = mirrorSources,
                SavePath = savePath
            };
            // 初始化SourceProvider移动到了Start方法里
            //var protocalProvider = new HttpProtocalProvider();
            //var mirrorSelector = new SequentialMirrorSelector();
            //mirrorSelector.Initialize(mainSource, mirrorSources);
            //this._sourceProvider = new SourceProvider(mirrorSelector, protocalProvider);

            // this._writer = // writer需要在Start的时候初始化
            this._maxThreadCount = threadCount;
        }
        #endregion

        #region properties
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
        #endregion

        #region public methods
        /// <summary>
        /// 开始下载
        /// </summary>
        public async void Start()
        {
            // 重置SourceProvider
            var protocalProvider = new HttpProtocalProvider();
            var mirrorSelector = new SequentialMirrorSelector();
            mirrorSelector.Initialize(this._localFileCache.MainSource, this._localFileCache.Mirrors);
            this._sourceProvider = new SourceProvider(mirrorSelector, protocalProvider);

            // 初始化SourceProvider
            await this._sourceProvider.InitializeAsync(CancellationToken.None);
            var newRemoteFileInfo = this._sourceProvider.GetRemoteFileInfo();

            // 对文件进行分片
            List<LocalSegment> segments = new List<LocalSegment>();
            if (this._localFileCache.HasSegment &&
                this._localFileCache.RemoteInfo != null &&
                RemoteFileInfo.IsSameFile(this._localFileCache.RemoteInfo, newRemoteFileInfo))
            {
                // 如果文件已经分片,服务器文件信息与缓存的文件信息一致则用缓存的文件分片
                segments.AddRange(this._localFileCache.Segments);
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
            this._writer = new LocalFileWriter(this._localFileCache.SavePath, segments);
            this._writer.CreateFile();

            // 创建下载线程
            this._threads = new List<SegmentThread>();
            for (int i = 0; i < this._maxThreadCount; i++)
            {
                var thread = new SegmentThread(i + 1, this._writer, this._sourceProvider);
                thread.Exited += Thread_Exited;
                this._threads.Add(thread);
            }

            // 启动下载线程
            foreach (var thread in this._threads)
            {
                thread.Start();
            }
        }

        private void Thread_Exited(object sender, ThreadExitedArgs e)
        {
            switch (e.Status)
            {
                case SegmentThreadStatuses.Idle:// 正常退出，说明这个线程已经完成了所有任务，并且没有新的任务了
                    {
                        if (this._writer.IsCompleted())
                        {
                            this._writer.Dispose();
                            this.OnCompleted();
                        }
                    }
                    break;
                case SegmentThreadStatuses.Running:// 在Thread_Exited方法中Status不可能为这个值，直接忽略
                    break;
                case SegmentThreadStatuses.Paused:// 线程被暂停了
                    break;
                case SegmentThreadStatuses.Failed:// 线程下载失败
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 暂停，可以调用Start继续下载
        /// </summary>
        public void Pause()
        {
            // 停止下载线程
            foreach (var thread in this._threads)
            {
                thread.Pause();
            }
            // 清空下载线程
            this._threads.Clear();
            // 缓存RemoteFileInfo
            this._localFileCache.RemoteInfo = this._sourceProvider.GetRemoteFileInfo();
            // 清空SourceProvider
            this._sourceProvider = null;
            // 缓存当前Writer信息，用来之后作为继续下载的依据
            this._localFileCache.Segments = this._writer.GetSegments();
            // 释放Writer
            this._writer.Dispose();
            this._writer = null;
        }

        /// <summary>
        /// 取消，会删除已经下载的文件
        /// </summary>
        public void Cancel()
        {
            this.Pause();
            // 删除本地文件
            File.Delete(this._localFileCache.SavePath);
            // 删除配置文件
        }
        #endregion

        #region private methdos
        #endregion
    }
}
