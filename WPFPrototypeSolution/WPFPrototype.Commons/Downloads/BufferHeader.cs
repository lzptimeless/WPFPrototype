using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 缓存头，记录数据在缓存中的位置
    /// </summary>
    public class BufferHeader
    {
        #region fields

        #endregion

        #region constructors
        public BufferHeader(int position, int length)
        {
            this._position = position;
            this._length = length;
        }
        #endregion

        #region properties
        #region Position
        private int _position;
        /// <summary>
        /// Get or set <see cref="Position"/>
        /// </summary>
        public int Position
        {
            get { return this._position; }
            set { this._position = value; }
        }
        #endregion

        #region Length
        private int _length;
        /// <summary>
        /// Get or set <see cref="Length"/>
        /// </summary>
        public int Length
        {
            get { return this._length; }
            set { this._length = value; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
