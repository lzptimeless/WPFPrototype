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
        /// 最小数据块的大小
        /// </summary>
        public const int MinBlockLength = 1024;
        /// <summary>
        /// 线程同步锁
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// 缓存的数据块
        /// </summary>
        private List<FileCacheBlock> _blocks;
        /// <summary>
        /// 缓存数据块长度
        /// </summary>
        private int _blockLength;
        #endregion

        #region constructors
        public FileCache(int length)
        {
            if (length < 0) throw new ArgumentException("length can not be less than 0.");

            this._blocks = new List<FileCacheBlock>();
            this._blockLength = Math.Min(length, Math.Max(MinBlockLength, length / 100)); // 默认分为一百个数据块
            this._totalLength = length;
        }
        #endregion

        #region properties
        #region TotalLength
        private int _totalLength;
        /// <summary>
        /// Get or set <see cref="TotalLength"/>，文件缓存总长度
        /// </summary>
        public int TotalLength
        {
            get { return this._totalLength; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 缓存数据，如果缓存成功返回true，如果缓存空间不足则返回false
        /// </summary>
        /// <param name="filePosition">数据对应文件的起始位置</param>
        /// <param name="buffer">数据buffer</param>
        /// <param name="bufferOffset">数据在buffer中的起始位置</param>
        /// <param name="length">数据长度</param>
        /// <returns></returns>
        public bool Cache(long filePosition, byte[] buffer, int bufferOffset, int length)
        {
            if (filePosition < 0) throw new ArgumentException("filePosition is can not less than 0.");
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (bufferOffset < 0) throw new ArgumentException("bufferOffset is can not less than 0.");
            if (length <= 0) throw new ArgumentException("length is can not less or equal than 0.");

            lock (this._syncRoot)
            {
                FileCacheBlock current;
                FileCacheBlock next;
                for (int i = 0; i <= this._blocks.Count; i++)
                {
                    current = i < this._blocks.Count ? this._blocks[i] : null;
                    next = i + 1 < this._blocks.Count ? this._blocks[i + 1] : null;
                    if (current == null)// 已经到达缓存数据结尾了
                    {
                        if (!this.CanCreateBlock()) return false; // 空间不足

                        var newBlock = this.CreateBlock(filePosition);
                        int writeLength = (int)Math.Min(newBlock.RemainLength, length);
                        newBlock.Write(buffer, bufferOffset, writeLength);
                        this._blocks.Add(newBlock);

                        filePosition += writeLength;
                        bufferOffset += writeLength;
                        length -= writeLength;

                        if (length > 0) continue;// 还剩余数据需要写入

                        return true;
                    }
                    else if (filePosition < current.FilePosition)// 数据起始点在当前数据块之前
                    {
                        if (!this.CanCreateBlock()) return false; // 空间不足

                        var newBlock = this.CreateBlock(filePosition);
                        int writeLength = (int)Math.Min(newBlock.RemainLength, Math.Min(current.FilePosition - filePosition, length));
                        newBlock.Write(buffer, bufferOffset, writeLength); // 写入数据
                        this._blocks.Insert(i, newBlock);// 插入新的缓存块

                        filePosition += writeLength;
                        bufferOffset += writeLength;
                        length -= writeLength;

                        if (length > 0) continue;// 还剩余数据需要写入

                        return true;
                    }
                    else if (filePosition <= (current.FilePosition + current.Position))// 数据起始点在当前数据块中或紧跟在当前数据块之后
                    {
                        if (filePosition + length <= current.FilePosition + current.Position) return true; // 数据完全重复了
                        // 去掉重复的部分
                        int cachedLength = (int)(current.FilePosition + current.Position - filePosition); // 重复的数据长度
                        filePosition += cachedLength;
                        bufferOffset += cachedLength;
                        length -= cachedLength;
                        // 写入数据到当前数据块
                        if (current.RemainLength > 0 && (next == null || filePosition < next.FilePosition))
                        {
                            int writeLength = Math.Min(current.RemainLength, length);
                            if (next != null) writeLength = (int)Math.Min(next.FilePosition - filePosition, writeLength);

                            current.Write(buffer, bufferOffset, writeLength);

                            filePosition += writeLength;
                            bufferOffset += writeLength;
                            length -= writeLength;
                        }

                        if (length > 0) continue;// 还有剩余数据需要写入

                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }// for
            }// block

            return false;
        }

        /// <summary>
        /// 将所有数据写入到本地文件
        /// </summary>
        /// <param name="writeFunc">写入委托</param>
        public void Flush(FileCacheWrite writeFunc)
        {
            lock (this._syncRoot)
            {
                foreach (var block in this._blocks)
                {
                    writeFunc(block.FilePosition, block.Buffer, 0, block.Position);
                }

                this._blocks.Clear();
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// 是否能够创建数据缓存块
        /// </summary>
        /// <returns></returns>
        private bool CanCreateBlock()
        {
            return this._blocks.Count * this._blockLength < this._totalLength;
        }

        /// <summary>
        /// 创建数据缓存块
        /// </summary>
        /// <returns></returns>
        private FileCacheBlock CreateBlock(long filePosition)
        {
            int currentTotalLength = this._blocks.Count * this._blockLength;
            if (currentTotalLength >= this._totalLength) throw new Exception("No space.");

            int blockLength = Math.Min(this._totalLength - currentTotalLength, this._blockLength);
            return new FileCacheBlock(filePosition, blockLength);
        }
        #endregion
    }

    /// <summary>
    /// 文件缓存写入到本地方法
    /// </summary>
    /// <param name="filePosition">数据对应文件的起始位置</param>
    /// <param name="buffer">数据buffer</param>
    /// <param name="bufferOffset">数据在buffer中的起始位置</param>
    /// <param name="length">数据长度</param>
    /// <returns></returns>
    public delegate void FileCacheWrite(long filePosition, byte[] buffer, int bufferOffset, int length);
}
