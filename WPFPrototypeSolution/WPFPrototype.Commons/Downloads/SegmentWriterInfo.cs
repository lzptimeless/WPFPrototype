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
    public class SegmentWriterInfo
    {
        #region fields

        #endregion

        #region constructors

        #endregion

        #region properties
        #region ThreadID
        private int _threadID;
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
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
