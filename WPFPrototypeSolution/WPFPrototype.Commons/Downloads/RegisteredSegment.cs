using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class RegisteredSegment
    {
        #region fields

        #endregion

        #region constructors
        public RegisteredSegment(long startPosition, long endPosition)
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
            get { return this._startPosition; }
            set { this._startPosition = value; }
        }
        #endregion

        #region EndPosition
        private long _endPosition;
        /// <summary>
        /// Get or set <see cref="EndPosition"/>
        /// </summary>
        public long EndPosition
        {
            get { return this._endPosition; }
            set { this._endPosition = value; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
