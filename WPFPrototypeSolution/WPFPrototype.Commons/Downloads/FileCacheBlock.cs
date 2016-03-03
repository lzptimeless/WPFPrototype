using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 文件缓存数据块
    /// </summary>
    public class FileCacheBlock
    {
        #region fields
        #endregion

        #region constructors
        public FileCacheBlock(long filePosition, int length)
        {
            this._filePosition = filePosition;
            this._buffer = new byte[length];
        }
        #endregion

        #region properties
        #region FilePosition
        private long _filePosition;
        /// <summary>
        /// Get or set <see cref="FilePosition"/>,在文件中的起始位置
        /// </summary>
        public long FilePosition
        {
            get { return this._filePosition; }
        }
        #endregion

        #region TotalLength
        /// <summary>
        /// Get or set <see cref="TotalLength"/>,自己缓存的总长度
        /// </summary>
        public int TotalLength
        {
            get { return this._buffer.Length; }
        }
        #endregion

        #region Position
        private int _position;
        /// <summary>
        /// Get or set <see cref="Position"/>,自己缓存的写入位置
        /// </summary>
        public int Position
        {
            get { return this._position; }
        }
        #endregion

        #region Length
        /// <summary>
        /// Get or set <see cref="Length"/>，已经存有数据的长度
        /// </summary>
        public int Length
        {
            get { return this._position; }
        }
        #endregion

        #region RemainLength
        /// <summary>
        /// Get or set <see cref="RemainLength"/>，空闲的长度
        /// </summary>
        public int RemainLength
        {
            get { return this._buffer.Length - this._position; }
        }
        #endregion

        #region Buffer
        private byte[] _buffer;
        /// <summary>
        /// Get or set <see cref="Buffer"/>,自己的缓存
        /// </summary>
        public byte[] Buffer
        {
            get { return this._buffer; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer">数据buffer</param>
        /// <param name="bufferOffset">数据在buffer中的起始位置</param>
        /// <param name="length">写入数据长度</param>
        public void Write(byte[] buffer, int bufferOffset, int length)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (bufferOffset < 0) throw new ArgumentException("bufferOffset is can not less than 0.");
            if (length <= 0) throw new ArgumentException("length is can not less or equal than 0.");
            if (this._buffer.Length - this._position < length) throw new Exception("Not enough space.");

            System.Buffer.BlockCopy(buffer, bufferOffset, this._buffer, this._position, length);
            this._position += length;
        }

        /// <summary>
        /// 重置这个数据块
        /// </summary>
        /// <param name="filePosition">新的文件起始位置</param>
        public void Reset(long filePosition)
        {
            this._filePosition = filePosition;
            this._position = 0;
        }
        #endregion

        #region private methods

        #endregion
    }
}
