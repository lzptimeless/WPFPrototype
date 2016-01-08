using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 通过数据源计算出来的片段
    /// </summary>
    public class CalculatedSegment
    {
        #region constructors
        public CalculatedSegment()
        { }

        public CalculatedSegment(long startPosition, long endPosition)
        {
            this._startPosition = startPosition;
            this._endPosition = endPosition;
        }
        #endregion

        #region properties
        #region StartPosition
        private long _startPosition;
        /// <summary>
        /// Get or set <see cref="StartPosition"/>
        /// </summary>
        public long StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }
        #endregion

        #region EndPosition
        private long _endPosition;
        /// <summary>
        /// Get or set <see cref="EndPosition"/>
        /// </summary>
        public long EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }
        #endregion
        #endregion
    }
}
