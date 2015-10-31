using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class Downloader
    {
        #region fields
        public const long SegmentMinSize = 1024 * 512;
        public const int DefaultThreadCount = 4;

        private ISegmentCalculator _segmentCalculator;
        private IProtocalProvider _protocalProvider;
        private IMirrorSelector _mirrorSelector;
        private int _threadCount;
        #endregion

        #region constructors
        public Downloader(LocalFileInfo localFile)
        {
            this.CheckLocalFileInfoValid(localFile);

            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);

            this._protocalProvider = new HttpProtocalProvider();
            this._protocalProvider.Init(this);

            this._mirrorSelector = new SequentialMirrorSelector();
            this._mirrorSelector.Init(this, localFile.Source, localFile.Mirrors);

            this._localFile = localFile;

            this._threadCount = DefaultThreadCount;
        }

        public Downloader(LocalFileInfo localFile, int threadCount)
        {
            this.CheckLocalFileInfoValid(localFile);

            this._segmentCalculator = new MinSizeSegmentCalculator(SegmentMinSize);

            this._protocalProvider = new HttpProtocalProvider();
            this._protocalProvider.Init(this);

            this._mirrorSelector = new SequentialMirrorSelector();
            this._mirrorSelector.Init(this, localFile.Source, localFile.Mirrors);

            this._localFile = localFile;

            this._threadCount = threadCount;
        }

        public Downloader(string url, string savePath)
           : this(url, null, savePath, DefaultThreadCount)
        { }

        public Downloader(string url, string savePath, int threadCount)
           : this(url, null, savePath, threadCount)
        { }

        public Downloader(string url, IEnumerable<string> mirrors, string savePath)
            : this(url, mirrors, savePath, DefaultThreadCount)
        { }

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
            this._protocalProvider.Init(this);

            this._mirrorSelector = new SequentialMirrorSelector();
            this._mirrorSelector.Init(this, source, mirrorSources);

            this._localFile = new LocalFileInfo();
            this._localFile.Source = source;
            this._localFile.Mirrors = mirrorSources;
            this._localFile.SavePath = savePath;
            this._localFile.CreateTime = DateTime.Now;

            this._threadCount = threadCount;
        }
        #endregion

        #region properties
        #region LocalFile
        private LocalFileInfo _localFile;
        /// <summary>
        /// Get or set <see cref="LocalFile"/>
        /// </summary>
        public LocalFileInfo LocalFile
        {
            get { return _localFile; }
        }
        #endregion

        #region Segments
        private List<Segment> _segments;
        /// <summary>
        /// Get or set <see cref="Segments"/>
        /// </summary>
        public IEnumerable<Segment> Segments
        {
            get { return _segments; }
        }
        #endregion
        #endregion

        #region public methods
        public async void Start()
        {
            var localFile = this._localFile;
            await this.PrepareRemoteInfo();

            if (!localFile.HasSegment)
            {
                this.CalculateSegment();
            }

            this.CreateFile();
        }

        public void Stop()
        { }
        #endregion

        #region private methdos
        private void CreateSegments()
        { }

        private void CreateFile()
        {
            var localFile = this._localFile;
            string path = localFile.SavePath;
            long size = localFile.RemoteInfo.Size;

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(size);
            }
        }

        private void CalculateSegment()
        {
            var localFile = this._localFile;
            var remoteInfo = localFile.RemoteInfo;

            if (!remoteInfo.IsAcceptRange)
            {
                localFile.Segments = new List<LocalSegment>();
                localFile.Segments.Add(
                    new LocalSegment
                    {
                        Index = 0,
                        StartPosition = 0,
                        EndPosition = remoteInfo.Size,
                        Position = 0
                    }
                );
                return;
            }

            var calculatedSegments = this._segmentCalculator.GetSegments(this._threadCount, localFile.RemoteInfo);

            List<LocalSegment> segments = new List<LocalSegment>();
            int index = 0;
            foreach (var calculateSegment in calculatedSegments)
            {
                segments.Add(
                    new LocalSegment
                    {
                        Index = index,
                        StartPosition = calculateSegment.StartPosition,
                        EndPosition = calculateSegment.EndPosition,
                        Position = 0
                    }
                );
                index++;
            }

            localFile.Segments = segments;
        }

        private async Task PrepareRemoteInfo()
        {
            var localFile = this._localFile;
            var remoteInfo = await this._protocalProvider.GetFileInfoAsync(this._localFile.Source);

            if (localFile.RemoteInfo == null ||
                (!RemoteFileInfo.IsSameFile(remoteInfo, localFile.RemoteInfo)) ||
                (!remoteInfo.IsAcceptRange))
            {
                localFile.Segments = null;
            }

            localFile.RemoteInfo = remoteInfo;
        }

        private void CheckLocalFileInfoValid(LocalFileInfo localFile)
        {
            if (localFile.Source == null) throw new Exception("LocalFileInfo.Source can not be null.");
            if (string.IsNullOrEmpty(localFile.SavePath)) throw new Exception("LocalFileInfo.SavePath can not be null or empty.");
        }
        #endregion
    }
}
