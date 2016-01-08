using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 镜像选择接口
    /// </summary>
    public interface IMirrorSelector:IEnumerable<FileSource>
    {
        /// <summary>
        /// 数据源
        /// </summary>
        FileSource Source { get; }

        /// <summary>
        /// 镜像数量
        /// </summary>
        int MirrorCount { get; }

        /// <summary>
        /// 初始化镜像
        /// </summary>
        /// <param name="downloader"><see cref="Downloader"/></param>
        /// <param name="source">原始数据源</param>
        /// <param name="mirrors">镜像数组</param>
        void Init(Downloader downloader, FileSource source, IEnumerable<FileSource> mirrors);

        /// <summary>
        /// 获取下一个数据源
        /// </summary>
        /// <returns></returns>
        FileSource GetNextSource();

        /// <summary>
        /// 移除这个镜像数据源
        /// </summary>
        /// <param name="mirror">镜像数据源</param>
        void Remove(FileSource mirror);
    }
}
