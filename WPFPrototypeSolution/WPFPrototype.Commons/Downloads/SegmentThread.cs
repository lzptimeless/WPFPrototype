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
        private Thread _thread;
        #endregion

        #region constructors
        public SegmentThread()
        {
            this._thread = new Thread(this.DownloadProc);
            // 为了跟随主线程关闭，这里必须设置为后台线程
            this._thread.IsBackground = true;
            // 这个名字只是方便调试观看
            this._thread.Name = "Downloader segment thread";
        }
        #endregion

        #region properties
        #region ID
        /// <summary>
        /// Get or set <see cref="ID"/>
        /// </summary>
        public int ID
        {
            get { return this._thread.ManagedThreadId; }
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
        private void DownloadProc()
        {
            try
            {

            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
