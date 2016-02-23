using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class ThreadExitedArgs : EventArgs
    {
        #region fields

        #endregion

        #region constructors
        public ThreadExitedArgs(SegmentThreadStatuses status)
        {
            this._status = status;
        }
        #endregion

        #region properties
        #region Status
        private SegmentThreadStatuses _status;
        /// <summary>
        /// Get or set <see cref="Status"/>
        /// </summary>
        public SegmentThreadStatuses Status
        {
            get { return this._status; }
            set { this._status = value; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
