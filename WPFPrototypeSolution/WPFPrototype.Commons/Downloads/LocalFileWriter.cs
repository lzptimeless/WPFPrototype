using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 标志是否已经被调用Dispose
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        /// 文件总大小
        /// </summary>
        private long _totalSize;
        /// <summary>
        /// 本地文件保存路径
        /// </summary>
        private string _savePath;
        /// <summary>
        /// 缓存文件片段
        /// </summary>
        private List<SegmentWriterInfo> _segmentInfos;
        /// <summary>
        /// 本地文件写入流
        /// </summary>
        private FileStream _stream;
        #endregion

        #region constructors
        public LocalFileWriter(LocalFileInfo info)
        {
            if (info == null) throw new ArgumentNullException("info");
            if (string.IsNullOrEmpty(info.SavePath)) throw new Exception("info.SavePath can not be empty.");
            if (info.RemoteInfo == null) throw new Exception("info.RemoteInfo can not be empty.");
            if (info.RemoteInfo.Size <= 0) throw new Exception("info.RemoteInfo.Size must bigger than 0.");

            this._savePath = info.SavePath;
            this._totalSize = info.RemoteInfo.Size;
            // 缓存文件片段
            this._segmentInfos = new List<SegmentWriterInfo>();

            if (info.Segments != null)
            {
                foreach (var segment in info.Segments)
                {
                    this._segmentInfos.Add(new SegmentWriterInfo
                    {
                        Segment = new LocalSegment
                        {
                            Index = segment.Index,
                            StartPosition = segment.StartPosition,
                            EndPosition = segment.EndPosition,
                            Position = segment.Position
                        }
                    });
                }// foreach
            }// if
        }
        #endregion

        #region properties

        #endregion

        #region public methods
        /// <summary>
        /// 创建本地文件，如果已经存在则不创建，设置文件大小
        /// </summary>
        public void CreateFile()
        {
            FileStream localStream = new FileStream(this._savePath, FileMode.OpenOrCreate, FileAccess.Write);

            lock (this._syncRoot)
            {
                // 有可能执行到这里的时候被调用了Dispose
                if (this._isDisposed)
                {
                    localStream.Dispose();
                    return;
                }

                this._stream = localStream;
            }

            if (this._stream.Length != this._totalSize)
            {
                this._stream.SetLength(this._totalSize);
            }
        }

        /// <summary>
        /// 写入数据到本地
        /// </summary>
        /// <param name="threadID">调用者的ID</param>
        /// <param name="positon">相对于文件片段的写入位置</param>
        /// <param name="buffer">数据buffer</param>
        /// <param name="bufferOffset">数据在buffer的起始位置</param>
        /// <param name="length">写入大小</param>
        public void Write(int threadID, long positon, byte[] buffer, int bufferOffset, int length)
        {
            if (this._stream == null) throw new Exception("LocalFileWriter not open local stream.");

            lock (this._stream)
            {
                if (this._stream.Position != positon)
                {
                    // 设置写入位置
                    this._stream.Seek(positon, SeekOrigin.Begin);
                }
                // 写入数据到本地
                this._stream.Write(buffer, bufferOffset, length);
            }// lock
        }

        public void Dispose()
        {
            if (this._syncRoot)
        }
        #endregion

        #region private methods
        private LocalSegment GetLocalSegment(int index)
        {

        }
        #endregion
    }
}
