using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 片段下载线程
    /// </summary>
    public class SegmentThread
    {
        #region fields
        private const int WriteBufferSize = 1024;

        private Thread _thread;
        private LocalFileWriter _writer;
        private IProtocalProvider _protocalProvider;
        private FileSource _source;
        private bool _isStop;
        #endregion

        #region constructors
        public SegmentThread(LocalFileWriter writer, IProtocalProvider protocalProvider)
        {
            this._thread = new Thread(this.ThreadProc);
            // 必须设置为Background，不然会导致无法重主线程结束程序
            this._thread.IsBackground = true;
            this._writer = writer;
            this._protocalProvider = protocalProvider;
        }
        #endregion

        #region properties
        #region StartPosition
        private long _startPosition;
        /// <summary>
        /// Get or set <see cref="StartPosition"/>，起始位置
        /// </summary>
        public long StartPosition
        {
            get { return this._startPosition; }
        }
        #endregion

        #region EndPosition
        private long _endPosition;
        /// <summary>
        /// Get or set <see cref="EndPosition"/>，结束为止
        /// </summary>
        public long EndPosition
        {
            get { return this._endPosition; }
            set { this._endPosition = value; }
        }
        #endregion

        #region Position
        private long _position;
        /// <summary>
        /// Get or set <see cref="Position"/>
        /// </summary>
        public long Position
        {
            get { return this._position; }
        }
        #endregion
        #endregion

        #region public methods
        public void Start()
        {
            this._isStop = false;
            this._thread.Start();
        }

        public void Stop()
        {
            this._isStop = true;
            this._thread.Abort();
        }
        #endregion

        #region private methods
        private async void ThreadProc()
        {
            // 初始化数据源
            if (!this.InitSource()) return;

            if (this._isStop) return; // 暂停下载

            // 下载
            while (true)
            {
                // 获取片段
                if (!this.InitSegment()) break;

                if (this._isStop) break; // 暂停下载

                // 下载这个片段
                await this.Download();

                if (this._isStop) break; // 暂停下载
            }
        }

        private bool InitSource()
        {
            // 如果已经初始化则不需要重复初始化
            if (this._source != null) return true;

            // 获取数据源
            while (true)
            {
                // 如果数据源错误则删除这个数据源
                break;
            }

            return false;
        }

        private bool InitSegment()
        {
            return false;
        }

        private async Task Download()
        {
            // 由于EndPosition可以被外部更改，可能引发多线程问题，需要缓存
            long oldEndPosition = this._endPosition;

            // 检测StartPosition和EndPosition是否正确
            if (this._startPosition > oldEndPosition) return;
            // 检测这个片段是否已经下载完
            if (this._startPosition + this._position > oldEndPosition) return;
            // 下载这个片段
            using (var stream = await this._protocalProvider.GetStreamAsync(this._source, this._startPosition + this._position, oldEndPosition))
            {
                if (this._isStop) return; // 暂停下载

                byte[] buffer = new byte[WriteBufferSize];
                int needReadLength = 0;
                int readLength = 0;
                oldEndPosition = this._endPosition;// 由于EndPosition可以被外部更改，可能引发多线程问题，需要缓存

                while (this._position <= oldEndPosition - this._startPosition)
                {
                    needReadLength = Math.Min(buffer.Length, (int)(oldEndPosition - this._startPosition + 1 - this._position));
                    readLength = stream.Read(buffer, 0, needReadLength);

                    if (this._isStop) break; // 暂停下载

                    this._writer.Write(buffer, this._position + this._startPosition, 0, readLength);
                    this._position += readLength;
                    oldEndPosition = this._endPosition;// 由于EndPosition可以被外部更改，可能引发多线程问题，需要缓存
                }// while
            } // using
        }
        #endregion
    }
}
