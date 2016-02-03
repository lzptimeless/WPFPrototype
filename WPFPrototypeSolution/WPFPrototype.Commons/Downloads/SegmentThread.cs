using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 循环获取下载起始字节，获取下载流，下载数据
    /// </summary>
    public class SegmentThread
    {
        #region fields
        public const int EmptyID = -1;

        private Thread _thread;
        private LocalFileWriter _writer;
        private SourceProvider _sourceProvider;
        #endregion

        #region constructors
        public SegmentThread(LocalFileWriter writer, SourceProvider sourceProvider)
        {
            this._thread = new Thread(this.DownloadProc);
            // 为了跟随主线程关闭，这里必须设置为后台线程
            this._thread.IsBackground = true;
            // 这个名字只是方便调试观看
            this._thread.Name = "Downloader segment thread";
            this._id = this._thread.ManagedThreadId;
            this._writer = writer;
            this._sourceProvider = sourceProvider;
        }
        #endregion

        #region properties
        #region ID
        private int _id;
        /// <summary>
        /// Get or set <see cref="ID"/>
        /// </summary>
        public int ID
        {
            get { return this._id; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            this._thread.Start();
        }

        /// <summary>
        /// 停止下载
        /// </summary>
        public void Stop()
        {
            this._thread.Abort();
        }
        #endregion

        #region private methods
        /// <summary>
        /// 下载线程
        /// </summary>
        private async void DownloadProc()
        {
            bool isRegisteredSegment = false; // 是否注册了片段
            try
            {
                while (true)// 循环下载文件片段
                {
                    var segment = this._writer.RegisterSegment(this._id); // 获取需要下载的片段
                    if (segment == null) break; // 注册的片段为null，证明下载已经完成或就快完成，这个线程已经不需要了
                    isRegisteredSegment = true;
                    // 下载片段
                    await this.DownloadSegment(segment);
                    // 取消注册下载片段
                    this._writer.UnregisterSegment(this._id);
                    isRegisteredSegment = false;
                }// while
            }
            catch (ThreadAbortException)
            {
                return;// ThreadAbortException表示下载停止
            }
            catch (Exception)
            {
                return;// 出现未知异常，停止下载
            }
            finally
            {
                // 取消注册片段
                if (isRegisteredSegment)
                {
                    this._writer.UnregisterSegment(this._id);
                }
                // 通知Downloader这个线程已经结束
            }
        }

        /// <summary>
        /// 下载指定的片段
        /// </summary>
        /// <param name="segment">要下载的片段</param>
        /// <returns></returns>
        private async Task DownloadSegment(CalculatedSegment segment)
        {
            long remainingLength = segment.EndPosition - segment.StartPosition + 1;// 当前片段剩余的长度
            long startPosition = segment.StartPosition;// 下载起始位置
            byte[] buffer = new byte[512]; // 下载数据的buffer
            int readLength = 0;// 当前从downloadStream读取的数据长度
            while (true) // 如果出现了异常需要继续尝试
            {
                try
                {
                    // 获取下载流
                    using (var downloadStream = await this._sourceProvider.GetStreamAsync(startPosition, startPosition + remainingLength - 1))
                    {
                        // 写入下载流
                        while (true)
                        {
                            readLength = downloadStream.Read(buffer, 0, buffer.Length); // 下载数据
                            if (readLength == 0) break; // 当前片段下载完成

                            remainingLength = this._writer.Write(this._id, buffer, 0, readLength);
                            if (remainingLength == 0) break; // 当前片段下载完成

                            startPosition += readLength;// 更新retry时的下载起始位置
                        }
                    }

                    break;// 没有发生异常则不需要重试
                }
                catch (ThreadAbortException)
                {
                    throw;// ThreadAbortException表示下载停止，应该直接抛出去
                }
                catch (Exception)
                {
                    continue;// 发生了其他异常，需要重试
                }
            }// while
        }
        #endregion
    }
}
