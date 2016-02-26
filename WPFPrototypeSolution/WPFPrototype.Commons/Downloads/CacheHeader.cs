using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 记录缓存在文件中的位置
    /// </summary>
    public class CacheHeader
    {
        #region fields
        #endregion

        #region constructors
        public CacheHeader(long filePosition)
        {
            this._startPosition = filePosition;
            this._bufferHeaders = new List<BufferHeader>();
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
        }
        #endregion

        #region Length
        private long _length;
        /// <summary>
        /// Get or set <see cref="Length"/>
        /// </summary>
        public long Length
        {
            get { return this._length; }
        }
        #endregion

        #region EndPosition
        /// <summary>
        /// Get or set <see cref="EndPosition"/>
        /// </summary>
        public long EndPosition
        {
            get { return this._startPosition + this._length - 1; }
        }
        #endregion

        #region BufferHeaders
        private List<BufferHeader> _bufferHeaders;
        /// <summary>
        /// Get or set <see cref="BufferHeaders"/>
        /// </summary>
        public IEnumerable<BufferHeader> BufferHeaders
        {
            get { return this._bufferHeaders; }
        }
        #endregion
        #endregion

        #region public methods
        public void AddBuffer(BufferHeader bufferHeader)
        {
            this._bufferHeaders.Add(bufferHeader);
            this._length += bufferHeader.Length;
        }

        public void RemoveBuffer(BufferHeader bufferHeader)
        {
            if (this._bufferHeaders.Remove(bufferHeader))
            {
                this._length -= bufferHeader.Length;
            }
        }
        #endregion

        #region private methods

        #endregion
    }
}
