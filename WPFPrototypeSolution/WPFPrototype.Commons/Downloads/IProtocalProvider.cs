using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 下载协议适配接口
    /// </summary>
    public interface IProtocalProvider
    {
        /// <summary>
        /// 获取数据源的下载流
        /// </summary>
        /// <param name="source">数据源<see cref="FileSource"/></param>
        /// <param name="startPosition">片段起始位置</param>
        /// <param name="endPosition">片段结束为止</param>
        /// <param name="ct">用来检测是否已经取消</param>
        /// <returns></returns>
        Task<Stream> GetStreamAsync(FileSource source, long startPosition, long endPosition, CancellationToken ct);

        /// <summary>
        /// 获取下载文件信息
        /// </summary>
        /// <param name="source">数据源<see cref="FileSource"/></param>
        /// <param name="ct">用来检测是否已经取消</param>
        /// <returns></returns>
        Task<RemoteFileInfo> GetFileInfoAsync(FileSource source, CancellationToken ct);
    }
}
