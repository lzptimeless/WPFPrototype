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
        public ThreadExitedArgs(long workID)
        {
            this._workID = workID;
        }
        #endregion

        #region properties
        #region WorkID
        private long _workID;
        /// <summary>
        /// Get or set <see cref="WorkID"/>
        /// </summary>
        public long WorkID
        {
            get { return this._workID; }
            set { this._workID = value; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
