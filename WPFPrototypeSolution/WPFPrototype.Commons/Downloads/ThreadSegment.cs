using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 存储LocalSegment与SegmentThread信息
    /// </summary>
    public class ThreadSegment
    {
        #region fields

        #endregion

        #region constructors

        #endregion

        #region properties
        #region ThreadID
        private int _threadID = SegmentThread.EmptyID;
        /// <summary>
        /// Get or set <see cref="ThreadID"/>
        /// </summary>
        public int ThreadID
        {
            get { return this._threadID; }
            set { this._threadID = value; }
        }
        #endregion

        #region Segment
        private LocalSegment _segment;
        /// <summary>
        /// Get or set <see cref="Segment"/>
        /// </summary>
        public LocalSegment Segment
        {
            get { return this._segment; }
            set { this._segment = value; }
        }
        #endregion

        #region IsCompleted
        /// <summary>
        /// Get or set <see cref="IsCompleted"/>，这个片段是否已经下载完成
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                var seg = this._segment;
                if (seg == null) return false;

                return seg.EndPosition - seg.StartPosition + 1 <= seg.Position;
            }
        }
        #endregion

        #region RemainingLength
        /// <summary>
        /// Get or set <see cref="RemainingLength"/>，剩余的还没有下载的长度
        /// </summary>
        public long RemainingLength
        {
            get
            {
                var seg = this._segment;
                if (seg == null) return 0;

                return Math.Max(0, seg.EndPosition - seg.StartPosition + 1 - seg.Position);
            }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
