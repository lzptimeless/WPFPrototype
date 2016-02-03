using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 分片策略
        /// </summary>
        private ISegmentCalculator _segmentCalculator;
        /// <summary>
        /// 下载协议
        /// </summary>
        private IProtocalProvider _protocalProvider;
        /// <summary>
        /// 镜像选择策略
        /// </summary>
        private IMirrorSelector _mirrorSelector;
        /// <summary>
        /// 下载线程数，代表最大可以同时下载的线程数
        /// </summary>
        private int _threadCount;
        /// <summary>
        /// 负责写入下载数据到本地
        /// </summary>
        private LocalFileWriter _writer;
        #endregion

        #region constructors
        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="localFile">本地文件</param>
        public Downloader(LocalFileInfo localFile)
        {
            this.CheckLocalFileInfoValid(localFile);

            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);

            this._protocalProvider = new HttpProtocalProvider();
            this._protocalProvider.Initialize(this);

            this._mirrorSelector = new SequentialMirrorSelector();
            this._mirrorSelector.Initialize(this, localFile.Source, localFile.Mirrors);

            this._localFile = localFile;
            this._writer = new LocalFileWriter(localFile);

            this._threadCount = DefaultThreadCount;
        }

        /// <summary>
        /// 创建<see cref="Downloader"/>
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="threadCount">下载线程数</param>
        public Downloader(LocalFileInfo localFile, int threadCount)
        {
            this.CheckLocalFileInfoValid(localFile);

            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);

            this._protocalProvider = new HttpProtocalProvider();
            this._protocalProvider.Initialize(this);

            this._mirrorSelector = new SequentialMirrorSelector();
            this._mirrorSelector.Initialize(this, localFile.Source, localFile.Mirrors);

            this._localFile = localFile;
            this._writer = new LocalFileWriter(localFile);

            this._threadCount = threadCount;
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
            var source = new FileSource(url);
            List<FileSource> mirrorSources = null;
            if (mirrors != null)
            {
                mirrorSources = new List<FileSource>();
                foreach (var mirror in mirrors)
                {
                    mirrorSources.Add(new FileSource(mirror));
                }
            }// if

            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);

            this._protocalProvider = new HttpProtocalProvider();
            this._protocalProvider.Initialize(this);

            this._mirrorSelector = new SequentialMirrorSelector();
            this._mirrorSelector.Initialize(this, source, mirrorSources);

            this._localFile = new LocalFileInfo();
            this._localFile.Source = source;
            this._localFile.Mirrors = mirrorSources;
            this._localFile.SavePath = savePath;
            this._writer = new LocalFileWriter(this._localFile);

            this._threadCount = threadCount;
        }
        #endregion

        #region properties
        #region LocalFile
        private LocalFileInfo _localFile;
        /// <summary>
        /// Get or set <see cref="LocalFile"/>，本地文件信息
        /// </summary>
        public LocalFileInfo LocalFile
        {
            get { return _localFile; }
        }
        #endregion

        #region Segments
        private List<SegmentThread> _segments;
        /// <summary>
        /// Get or set <see cref="Segments"/>，分片信息
        /// </summary>
        public IEnumerable<SegmentThread> Segments
        {
            get { return _segments; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 开始下载
        /// </summary>
        public async void Start()
        {
            var localFile = this._localFile;
            // 获取下载文件信息
            await this.PrepareRemoteInfo();

            // 如果还没有分片则将文件分片
            if (!localFile.HasSegment)
            {
                this.CalculateSegment();
            }

            // 创建本地文件
            this._writer.CreateFile();

            // 创建下载线程
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Stop()
        { }
        #endregion

        #region private methdos
        private void CreateSegmentThreads()
        {
            
        }

        /// <summary>
        /// 根据下载文件信息对文件进行分片，如果不支持分片下载则只会有一个片段
        /// </summary>
        private void CalculateSegment()
        {
            var localFile = this._localFile;
            var remoteInfo = localFile.RemoteInfo;

            if (!remoteInfo.IsAcceptRange)
            {
                // 如果不支持分片下载则只分一个片段
                localFile.Segments = new List<LocalSegment>();
                localFile.Segments.Add(
                    new LocalSegment
                    {
                        StartPosition = 0,
                        EndPosition = remoteInfo.Size - 1,
                        Position = 0
                    }
                );
                return;
            }

            // 计算分片
            var calculatedSegments = this._segmentCalculator.GetSegments(this._threadCount, localFile.RemoteInfo);

            // 初始化分片
            List<LocalSegment> segments = new List<LocalSegment>();
            foreach (var calculateSegment in calculatedSegments)
            {
                segments.Add(
                    new LocalSegment
                    {
                        StartPosition = calculateSegment.StartPosition,
                        EndPosition = calculateSegment.EndPosition,
                        Position = 0
                    }
                );
            }

            localFile.Segments = segments;
        }

        /// <summary>
        /// 获取下载文件信息，如果本地文件与最新信息不符则重置本地文件
        /// </summary>
        /// <returns></returns>
        private async Task PrepareRemoteInfo()
        {
            var localFile = this._localFile;
            var oldRemoteInfo = localFile.RemoteInfo;
            var newRemoteInfo = await this._protocalProvider.GetFileInfoAsync(this._localFile.Source);

            if (oldRemoteInfo == null || !RemoteFileInfo.IsSameFile(oldRemoteInfo, newRemoteInfo))
            {
                localFile.Segments = null;
            }

            localFile.RemoteInfo = newRemoteInfo;
        }

        /// <summary>
        /// 检查本地文件配置是否正确
        /// </summary>
        /// <param name="localFile"><see cref="LocalFileInfo"/></param>
        private void CheckLocalFileInfoValid(LocalFileInfo localFile)
        {
            if (localFile.Source == null) throw new Exception("LocalFileInfo.Source can not be null.");
            if (string.IsNullOrEmpty(localFile.SavePath)) throw new Exception("LocalFileInfo.SavePath can not be null or empty.");
        }
        #endregion
    }
}
