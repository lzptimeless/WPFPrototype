using WPFPrototype.Toolkit.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Toolkit.Languages
{
    /// <summary>
    /// 多语言辅助类
    /// </summary>
    public static class LanguageHelper
    {
        #region fields
        private static ILanguageResources _resources;
        #endregion

        #region constructors
        static LanguageHelper()
        {
            if (View.IsDesignMode)
            {
                // 在设计状态下只能通过Resource模式加载多语言资源
                _resources = new LanguageResources("/WPFPrototype.Toolkit;component/Assets/Languages/", LanguageResourceTypes.Resource);
            }
            else
            {
                // 通过Content模式加载多语言资源
                _resources = new LanguageResources("/Languages/", LanguageResourceTypes.Content);
                // 通过File模式加载多语言资源
                //_resources = new LanguageResources("Languages/", LanguageResourceTypes.File);
            }
            
            _resources.SetLanguage("en-us");
        }
        #endregion

        #region properties
        /// <summary>
        /// 多语言资源
        /// </summary>
        public static ILanguageResources Resources
        {
            get { return _resources; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// 获取多语言文字，可能为null
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static string GetLanguage(string key)
        {
            return _resources[key];
        }
        #endregion
    }
}
