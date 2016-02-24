using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 下载器状态
    /// </summary>
    public enum DownloaderStatuses
    {
        /// <summary>
        /// 空闲，初始化完成还没有开始下载
        /// </summary>
        Idle,
        /// <summary>
        /// 正在下载
        /// </summary>
        Downloading,
        /// <summary>
        /// 下载完成
        /// </summary>
        Completed,
        /// <summary>
        /// 已经暂停
        /// </summary>
        Paused,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled,
        /// <summary>
        /// 下载失败
        /// </summary>
        Failed
    }
}
