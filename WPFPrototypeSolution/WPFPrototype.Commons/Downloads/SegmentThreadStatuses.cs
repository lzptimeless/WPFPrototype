using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 下载线程状态
    /// </summary>
    public enum SegmentThreadStatuses
    {
        /// <summary>
        /// 初始化完成，还没有运行或现在完成
        /// </summary>
        Idle,
        /// <summary>
        /// 正在运行
        /// </summary>
        Running,
        /// <summary>
        /// 被取消了
        /// </summary>
        Paused,
        /// <summary>
        /// 下载失败
        /// </summary>
        Failed
    }
}
