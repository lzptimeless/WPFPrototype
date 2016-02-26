using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 文件缓存，因为下载数据比较零散，写入也比较频繁，所以效率低，使用文件缓存可以增加效率
    /// </summary>
    public class FileCache
    {
        #region fields
        /// <summary>
        /// 用于线程同步
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// 文件缓存的Buffer
        /// </summary>
        private byte[] _buffer;
        /// <summary>
        /// 文件缓存写入指针
        /// </summary>
        private int _position;
        /// <summary>
        /// 记录文件数据在缓存中的映射关系
        /// </summary>
        private List<CacheHeader> _cacheHeaders;
        #endregion

        #region constructors
        public FileCache(int length)
        {
            this._buffer = new byte[length];
            this._cacheHeaders = new List<CacheHeader>();
        }
        #endregion

        #region properties
        #region TotalLength
        /// <summary>
        /// Get or set <see cref="TotalLength"/>
        /// </summary>
        public int TotalLength
        {
            get { return this._buffer.Length; }
        }
        #endregion

        #region CachedLength
        /// <summary>
        /// Get or set <see cref="CachedLength"/>
        /// </summary>
        public int CachedLength
        {
            get { return this._position; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 缓存文件数据
        /// </summary>
        /// <param name="filePosition">数据在文件中的起始位置</param>
        /// <param name="buffer">数据buffer</param>
        /// <param name="bufferOffset">数据在buffer中的起始位置</param>
        /// <param name="length">缓存数据长度</param>
        /// <returns></returns>
        public bool Cache(long filePosition, byte[] buffer, int bufferOffset, int length)
        {
            lock (this._syncRoot)
            {
                if (this._buffer.Length < this._position + length) return false; // 剩余缓存空间不足

                int insertIndex = -1; // 插入位置
                long startPosition = filePosition;// 新的片段的起始位置
                long bufferedPosition = filePosition; // 缓存指针
                List<BufferHeader> bufferHeaders = new List<BufferHeader>(); // 新片段的缓存头
                List<CacheHeader> preRemoveHeaders = new List<CacheHeader>(); // 预计要移除的片段（这些片段被新插入的片段合并了）
                CacheHeader tmpHeader;// 用于遍历的临时变量
                for (int i = 0; i < this._cacheHeaders.Count; i++)
                {
                    tmpHeader = this._cacheHeaders[i];
                    if (tmpHeader.EndPosition + 1 < filePosition) continue; // 在i的后面

                    if (insertIndex == -1) insertIndex = i; // 记录插入位置，无论是在i的前面，相交，紧随，最终i都要被合并的

                    if (filePosition + length < tmpHeader.StartPosition) // 在i的前面，表明与剩余的片段都没关系了，停止遍历
                    {
                        if ()
                        break;
                    }

                    // 以下就是与i相交的情况了
                    if (filePosition < tm)

                }// for

                // 缓存数据
                //Buffer.BlockCopy(buffer, bufferOffset, this._buffer, this._position, bufferHeader.Length);
                //this._position += bufferHeader.Length;
            }// lock

            return true;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            lock(this._syncRoot)
            {
                this._position = 0;
                this._cacheHeaders.Clear();
            }
        }
        #endregion

        #region private methods

        #endregion
    }
}
