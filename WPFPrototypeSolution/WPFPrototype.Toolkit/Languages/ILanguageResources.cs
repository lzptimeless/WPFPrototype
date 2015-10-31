using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Toolkit.Languages
{
    /// <summary>
    /// 多语言资源接口，提供文字查询
    /// </summary>
    public interface ILanguageResources
    {
        /// <summary>
        /// 获取资源文件路径
        /// </summary>
        string ResourcePath { get; }

        /// <summary>
        /// 获取多语言资源类型
        /// </summary>
        LanguageResourceTypes ResourceType { get; }

        /// <summary>
        /// 获取对应键值的文字，不存在则返回null
        /// </summary>
        /// <param name="key">多语言键值</param>
        /// <returns></returns>
        string this[string key] { get; }

        /// <summary>
        /// 检查默认路径下的资源是否支持这个语言，只要找到相近的语言都算作支持
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言</param>
        /// <returns></returns>
        bool IsSupported(string ietfLanguageTag);

        /// <summary>
        /// 检查当前语言是否在默认路径下存在
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言</param>
        /// <returns></returns>
        bool IsContains(string ietfLanguageTag);

        /// <summary>
        /// 获取默认资源文件夹下所有的语言，返回ietf language tag表示的语言
        /// </summary>
        /// <returns></returns>
        string[] GetLanguages();

        /// <summary>
        /// 获取当前语言，返回ietf language tag表示的语言
        /// </summary>
        /// <returns></returns>
        string GetLanguage();

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言，用来查找默认路径下多语言资源文件</param>
        void SetLanguage(string ietfLanguageTag);

        /// <summary>
        /// 语言改变事件
        /// </summary>
        event EventHandler LanguageChangedEvent;
    }
}
