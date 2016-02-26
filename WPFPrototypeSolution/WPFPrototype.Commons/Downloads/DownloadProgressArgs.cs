using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class DownloadProgressArgs : EventArgs
    {
        #region fields

        #endregion

        #region constructors
        public DownloadProgressArgs(List<LocalSegment> segments)
        {
            foreach (var segment in segments)
            {
                this._totalSize += segment.EndPosition - segment.StartPosition + 1;
                this._downloadedSize += segment.Position;
            }

            if (this._totalSize > 0)
            {
                this._progress = (int)(this._downloadedSize * 100 / this._totalSize);
            }
            this._segments = segments;
        }
        #endregion

        #region properties
        #region TotalSize
        private long _totalSize;
        /// <summary>
        /// Get or set <see cref="TotalSize"/>
        /// </summary>
        public long TotalSize
        {
            get { return this._totalSize; }
            set { this._totalSize = value; }
        }
        #endregion

        #region DownloadedSize
        private long _downloadedSize;
        /// <summary>
        /// Get or set <see cref="DownloadedSize"/>
        /// </summary>
        public long DownloadedSize
        {
            get { return this._downloadedSize; }
            set { this._downloadedSize = value; }
        }
        #endregion

        #region Progress
        private int _progress;
        /// <summary>
        /// Get or set <see cref="Progress"/>
        /// </summary>
        public int Progress
        {
            get { return this._progress; }
            set { this._progress = value; }
        }
        #endregion

        #region Segments
        private List<LocalSegment> _segments;
        /// <summary>
        /// Get or set <see cref="Segments"/>
        /// </summary>
        public List<LocalSegment> Segments
        {
            get { return this._segments; }
            set { this._segments = value; }
        }
        #endregion

        #region Speed
        private long _speed;
        /// <summary>
        /// Get or set <see cref="Speed"/>，每秒下载字节
        /// </summary>
        public long Speed
        {
            get { return this._speed; }
            set { this._speed = value; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
