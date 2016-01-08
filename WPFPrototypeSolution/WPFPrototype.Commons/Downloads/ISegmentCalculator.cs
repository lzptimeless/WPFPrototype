using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 下载文件分段策略接口
    /// </summary>
    public interface ISegmentCalculator
    {
        /// <summary>
        /// 获取下载文件分段
        /// </summary>
        /// <param name="count">分段数量</param>
        /// <param name="fileInfo">下载文件信息</param>
        /// <returns></returns>
        CalculatedSegment[] GetSegments(int count, RemoteFileInfo fileInfo);
    }
}
