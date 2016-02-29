using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 对本地文件进行创建，分配片段，写入
    /// </summary>
    public class LocalFileWriter : IDisposable
    {
        #region fields
        /// <summary>
        /// 用于这个对象内部lock
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// 标志是否已经调用Dispose
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        /// 本地文件保存路径
        /// </summary>
        private string _savePath;
        /// <summary>
        /// 缓存文件片段
        /// </summary>
        private List<ThreadSegment> _segments;
        /// <summary>
        /// 本地文件写入流
        /// </summary>
        private FileStream _stream;
        /// <summary>
        /// 数据缓存，由于写入请求很分散很多，直接写入到文件效率很低，所以使用缓存
        /// </summary>
        private FileCache _cache;
        #endregion

        #region constructors
        public LocalFileWriter(string savePath, List<LocalSegment> localSegments, int cacheSize)
        {
            if (string.IsNullOrEmpty(savePath)) throw new Exception("savePath can not be empty.");
            if (localSegments == null) throw new ArgumentNullException("localSegments");
            if (localSegments.Count == 0) throw new Exception("localSegments can not be empty.");

            this._savePath = savePath;
            // 缓存文件片段
            this._segments = new List<ThreadSegment>();
            foreach (var segment in localSegments)
            {
                this._segments.Add(new ThreadSegment
                {
                    // 有必要重新创建LocalSegment，防止被外部修改
                    Segment = new LocalSegment
                    {
                        StartPosition = segment.StartPosition,
                        EndPosition = segment.EndPosition,
                        Position = segment.Position
                    }
                });
            }// foreach

            this._cache = new FileCache(cacheSize);
        }
        #endregion

        #region properties

        #endregion

        #region events
        #endregion

        #region public methods
        /// <summary>
        /// 获取当前文件片段的下载状态
        /// </summary>
        /// <returns></returns>
        public List<LocalSegment> GetSegments()
        {
            List<LocalSegment> localSegments = new List<LocalSegment>();

            lock (this._syncRoot)
            {
                LocalSegment innerSegment;
                foreach (var segment in this._segments)
                {
                    innerSegment = segment.Segment;
                    // 复制LocalSegment，防止被外部操作意外修改
                    localSegments.Add(new LocalSegment
                    {
                        StartPosition = innerSegment.StartPosition,
                        EndPosition = innerSegment.EndPosition,
                        Position = innerSegment.Position
                    });
                }
            }

            return localSegments;
        }

        /// <summary>
        /// 获取当前已经下载的
        /// </summary>
        /// <returns></returns>
        public long GetDownloadedSize()
        {
            long downloadedSize = 0;

            lock (this._syncRoot)
            {
                foreach (var segment in this._segments)
                {
                    downloadedSize += segment.Segment.Position;
                }
            }

            return downloadedSize;
        }

        /// <summary>
        /// 创建本地文件，如果已经存在则不创建，设置文件大小
        /// </summary>
        public void CreateFile()
        {
            lock (this._syncRoot)
            {
                this.ThrowIfDisposed();

                var firstSegment = this._segments.First();
                var lastSegment = this._segments.Last();
                long totalSize = lastSegment.Segment.EndPosition - firstSegment.Segment.StartPosition + 1;
                FileStream localStream = new FileStream(this._savePath, FileMode.OpenOrCreate, FileAccess.Write);
                if (localStream.Length != totalSize)
                {
                    localStream.SetLength(totalSize);
                }
                this._stream = localStream;
            }
        }

        /// <summary>
        /// 获取文件是否已经下载完成
        /// </summary>
        /// <returns></returns>
        public bool IsCompleted()
        {
            lock (this._syncRoot)
            {
                foreach (var segment in this._segments)
                {
                    if (!segment.IsCompleted) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 注册一个片段来下载这段数据，返回需要下载的片段
        /// </summary>
        /// <param name="threadID">注册的线程ID</param>
        /// <returns>需要下载的片段</returns>
        public RegisteredSegment RegisterSegment(int threadID)
        {
            lock (this._syncRoot)
            {
                this.ThrowIfDisposed();

                // 先查找这个线程是否已经注册过片段
                var oldSegment = this.GetThreadSegment(threadID);
                if (oldSegment != null) throw new Exception(string.Format("The thread:{0} has been registered, should call Unregister before this operation.", threadID));

                // 优先获取没有注册的片段
                var newSegment = this.GetFirstUnregisteredUncompletedThreadSegment();
                // 如果都被注册了就重新分出一个片段来
                if (newSegment == null)
                {
                    newSegment = this.SliceThreadSegment();
                }
                // 注册这个片段
                if (newSegment != null)
                {
                    newSegment.ThreadID = threadID;
                    var innerSegment = newSegment.Segment;
                    return new RegisteredSegment(innerSegment.StartPosition + innerSegment.Position, innerSegment.EndPosition);
                }

                return null;
            }// lock
        }

        /// <summary>
        /// 取消注册片段，一般这个片段数据下载完成时调用。如果没有注册过，则没有影响
        /// </summary>
        /// <param name="threadID">需要注册的线程ID</param>
        public void UnregisterSegment(int threadID)
        {
            lock (this._syncRoot)
            {
                this.ThrowIfDisposed();

                // 先查找这个线程是否已经注册过片段
                var segment = this.GetThreadSegment(threadID);
                if (segment == null) return;

                segment.ThreadID = SegmentThread.EmptyID;
            }
        }

        /// <summary>
        /// 写入数据到本地，返回这个片段剩余的长度
        /// </summary>
        /// <param name="threadID">调用者的ID</param>
        /// <param name="buffer">数据buffer</param>
        /// <param name="bufferOffset">数据在buffer的起始位置</param>
        /// <param name="length">写入大小</param>
        /// <returns>这个片段剩余的长度</returns>
        public long Write(int threadID, byte[] buffer, int bufferOffset, int length)
        {
            lock (this._syncRoot)
            {
                this.ThrowIfDisposed(); // 检测是否已经释放

                if (this._stream == null) throw new Exception("Not open local stream.");

                var threadSegment = this.GetThreadSegment(threadID); // 获取线程对应的片段
                if (threadSegment == null) throw new Exception(string.Format("The thread:{0} not registered.", threadID));

                if (threadSegment.IsCompleted) return 0;

                var innerSegment = threadSegment.Segment;
                long offset = innerSegment.StartPosition + innerSegment.Position; // 计算真实写入offset
                int writeLength = (int)Math.Min(length, threadSegment.RemainingLength); // 计算写入长度
                bool cacheSuccess = this._cache.Cache(offset, buffer, bufferOffset, writeLength);

                if (!cacheSuccess)
                {
                    this._cache.Flush(this.WriteToFile);
                    cacheSuccess = this._cache.Cache(offset, buffer, bufferOffset, writeLength);

                    if (!cacheSuccess) throw new Exception("Cache failed.");
                }
                
                innerSegment.Position += writeLength; // 更新写入位置

                return threadSegment.RemainingLength;
            }// lock
        }

        public void Dispose()
        {
            lock (this._syncRoot)
            {
                this._isDisposed = true;
                if (this._stream != null)
                {
                    this._stream.Dispose();
                    this._stream = null;
                }
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// 写入数据到本地方法
        /// </summary>
        /// <param name="filePosition">数据对应文件的起始位置</param>
        /// <param name="buffer">数据buffer</param>
        /// <param name="bufferOffset">数据在buffer中的起始位置</param>
        /// <param name="length">数据长度</param>
        /// <returns></returns>
        private void WriteToFile(long filePosition, byte[] buffer, int bufferOffset, int length)
        {
            if (this._stream.Position != filePosition)
            {
                // 设置写入位置
                this._stream.Seek(filePosition, SeekOrigin.Begin);
            }

            // 写入数据到本地
            this._stream.Write(buffer, bufferOffset, length);
        }

        /// <summary>
        /// 通过ThreadID获取ThreadSegment
        /// </summary>
        /// <param name="threadID"><see cref="SegmentThread"/>的ID</param>
        /// <returns></returns>
        private ThreadSegment GetThreadSegment(int threadID)
        {
            foreach (var segment in this._segments)
            {
                if (segment.ThreadID == threadID) return segment;
            }

            return null;
        }

        /// <summary>
        /// 获取第一个没有登记的和没有完成下载的片段
        /// </summary>
        /// <returns></returns>
        private ThreadSegment GetFirstUnregisteredUncompletedThreadSegment()
        {
            foreach (var segment in this._segments)
            {
                if (segment.ThreadID == SegmentThread.EmptyID && !segment.IsCompleted) return segment;
            }

            return null;
        }

        /// <summary>
        /// 从剩下的没有下载完成的最大的片段中分出一个片段
        /// </summary>
        /// <returns></returns>
        private ThreadSegment SliceThreadSegment()
        {
            int maxSegmentIndex = -1; // 剩余最大片段的索引
            long maxRemainingLength = 0; // 剩余的最大片段的剩余长度
            long remainingLength = 0; // 临时变量
            // 查找剩余最大的片段
            for (int i = 0; i < this._segments.Count; i++)
            {
                remainingLength = this._segments[i].RemainingLength;
                if (maxSegmentIndex == -1)
                {
                    if (remainingLength > 1024 * 1024)
                    {
                        maxSegmentIndex = i;
                        maxRemainingLength = remainingLength;
                    }
                }
                else
                {
                    if (remainingLength > maxRemainingLength)
                    {
                        maxSegmentIndex = i;
                        maxRemainingLength = remainingLength;
                    }
                }
            }// foreach

            if (maxSegmentIndex == -1) return null;

            ThreadSegment maxSegment = this._segments[maxSegmentIndex];// 剩余最大片段
            long newSegmentLength = maxRemainingLength / 2;// 新分出来的片段长度
            ThreadSegment newSegment = new ThreadSegment
            {
                Segment = new LocalSegment
                {
                    StartPosition = maxSegment.Segment.EndPosition - newSegmentLength + 1,
                    EndPosition = maxSegment.Segment.EndPosition
                }
            };
            // 修改被分割片段的结束字节位置
            maxSegment.Segment.EndPosition = newSegment.Segment.StartPosition - 1;
            // 插入新的片段到数组
            this._segments.Insert(maxSegmentIndex + 1, newSegment);

            return newSegment;
        }

        /// <summary>
        /// 如果this._isDisposed == true就抛出异常ObjectDisposedException
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this._isDisposed) throw new ObjectDisposedException(this.GetType().Name);
        }
        #endregion
    }
}
