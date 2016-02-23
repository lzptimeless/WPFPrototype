using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 循环获取下载起始字节，获取下载流，下载数据
    /// </summary>
    public class SegmentThread
    {
        #region fields
        /// <summary>
        /// 空的线程ID
        /// </summary>
        public const int EmptyID = -1;
        /// <summary>
        /// 用于lock操作
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// 写入到本地操作管理
        /// </summary>
        private LocalFileWriter _writer;
        /// <summary>
        /// 网络数据获取管理
        /// </summary>
        private SourceProvider _sourceProvider;
        /// <summary>
        /// 用于取消下载
        /// </summary>
        private CancellationTokenSource _cts;
        #endregion

        #region constructors
        public SegmentThread(int id, LocalFileWriter writer, SourceProvider sourceProvider)
        {
            this._id = id;
            this._writer = writer;
            this._sourceProvider = sourceProvider;
            this._status = SegmentThreadStatuses.Idle;
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

        #region Status
        private SegmentThreadStatuses _status;
        /// <summary>
        /// Get or set <see cref="Status"/>
        /// </summary>
        public SegmentThreadStatuses Status
        {
            get { return this._status; }
        }
        #endregion
        #endregion

        #region events
        #region Exited
        /// <summary>
        /// Event name of <see cref="Exited"/>
        /// </summary>
        public const string ExitedEventName = "Exited";

        public event EventHandler<ThreadExitedArgs> Exited;

        private void OnExited(ThreadExitedArgs e)
        {
            EventHandler<ThreadExitedArgs> handler = this.Exited;

            if (handler != null) handler(this, e);
        }

        public void AddWeakExitedHandler(EventHandler<ThreadExitedArgs> handler)
        {
            WeakEventManager<SegmentThread, ThreadExitedArgs>.AddHandler(this, ExitedEventName, handler);
        }

        public void RemoveWeakExitedHandler(EventHandler<ThreadExitedArgs> handler)
        {
            WeakEventManager<SegmentThread, ThreadExitedArgs>.RemoveHandler(this, ExitedEventName, handler);
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            var cts = new CancellationTokenSource();
            lock (this._syncRoot)
            {
                if (this._cts != null)
                {
                    cts.Dispose();
                    throw new Exception("The thread is running.");
                }
                this._cts = cts;
                this._status = SegmentThreadStatuses.Running;
            }

            Task.Run(() => { return this.DownloadProc(cts); }, cts.Token);
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            lock (this._syncRoot)
            {
                if (this._cts == null) return;

                this._cts.Cancel();
                this._cts = null;
                this._writer.UnregisterSegment(this._id);
                this._status = SegmentThreadStatuses.Paused;
                this.OnExited(new ThreadExitedArgs(this._status));
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// 下载线程
        /// </summary>
        /// <param name="cts">检测是否取消</param>
        /// <returns></returns>
        private async Task DownloadProc(CancellationTokenSource cts)
        {
            var ct = cts.Token;
            try
            {
                while (true)// 循环下载文件片段
                {
                    RegisteredSegment segment;
                    lock (this._syncRoot)
                    {
                        ct.ThrowIfCancellationRequested(); // 检测是否取消下载
                        segment = this._writer.RegisterSegment(this._id); // 获取需要下载的片段
                    }

                    if (segment == null) break; // 注册的片段为null，证明下载已经完成或就快完成，这个线程已经获取不到任务了

                    await this.DownloadSegment(segment, ct);// 下载片段

                    lock (this._syncRoot)
                    {
                        ct.ThrowIfCancellationRequested();
                        this._writer.UnregisterSegment(this._id);// 取消注册下载片段
                    }
                }// while

                // 设置线程状态为空闲
                lock (this._syncRoot)
                {
                    ct.ThrowIfCancellationRequested();
                    this._status = SegmentThreadStatuses.Idle;
                }
            }
            catch (OperationCanceledException)
            {
                // 线程状态在Stop方法中被设置为了Cancelled，这里就不用设置了
                return;// 表示取消
            }
            catch (Exception)
            {
                // 设置线程状态为失败
                lock (this._syncRoot)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        this._status = SegmentThreadStatuses.Failed;
                    }
                }
                return;// 出现未知异常，停止下载
            }
            finally
            {
                // 取消注册片段
                lock(this._syncRoot)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        this._cts = null;
                        this._writer.UnregisterSegment(this._id);
                        this.OnExited(new ThreadExitedArgs(this._status));// 通知Downloader这个线程已经结束
                    }
                }
                cts.Dispose();// 释放取消资源
            }
        }

        /// <summary>
        /// 下载指定的片段
        /// </summary>
        /// <param name="segment">要下载的片段</param>
        /// <param name="ct">检测是否取消</param>
        /// <returns></returns>
        private async Task DownloadSegment(RegisteredSegment segment, CancellationToken ct)
        {
            long remainingLength = segment.EndPosition - segment.StartPosition + 1;// 当前片段剩余的长度
            long startPosition = segment.StartPosition;// 下载起始位置
            byte[] buffer = new byte[512]; // 下载数据的buffer
            int readLength = 0;// 当前从downloadStream读取的数据长度
            bool isWriteError = false; // 是否在写入本地时发生了异常
            while (true) // 如果出现了异常需要继续尝试
            {
                ct.ThrowIfCancellationRequested(); // 检测是否取消下载
                // 获取下载流
                using (var downloadStream = await this._sourceProvider.GetStreamAsync(startPosition, startPosition + remainingLength - 1, ct))
                {
                    try
                    {
                        // 写入下载流
                        while (true)
                        {
                            ct.ThrowIfCancellationRequested(); // 检测是否取消下载

                            readLength = await downloadStream.ReadAsync(buffer, 0, buffer.Length, ct); // 下载数据
                            if (readLength == 0) throw new Exception("Read network data failed."); // 读取数据失败

                            lock(this._syncRoot)
                            {
                                ct.ThrowIfCancellationRequested();
                                try
                                {
                                    remainingLength = this._writer.Write(this._id, buffer, 0, readLength);
                                }
                                catch
                                {
                                    isWriteError = true;
                                    throw;
                                }
                            }// lock
                            if (remainingLength == 0) break; // 当前片段下载完成

                            startPosition += readLength;// 更新retry时的下载起始位置
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;// 表示取消下载
                    }
                    catch (Exception)
                    {
                        if (isWriteError) throw; // 如果写入操作发生异常者直接退出

                        continue;// 发生了网络相关的异常，需要重试
                    }
                }// using

                break;// 没有发生异常则不需要重试
            }// while
        }
        #endregion
    }
}
