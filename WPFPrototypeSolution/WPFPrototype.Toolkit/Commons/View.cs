using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFPrototype.Toolkit.Commons
{
    /// <summary>
    /// View相关的辅助属性和方法
    /// </summary>
    public static class View
    {
        #region fields
        private static bool _isDesignMode;
        #endregion

        #region constructors
        static View()
        {
           _isDesignMode = DesignerProperties.GetIsInDesignMode(new ContentControl());
        }
        #endregion

        #region properties
        /// <summary>
        /// 获取是否是设计状态
        /// </summary>
        public static bool IsDesignMode
        {
            get { return _isDesignMode; }
        }
        #endregion
    }
}
