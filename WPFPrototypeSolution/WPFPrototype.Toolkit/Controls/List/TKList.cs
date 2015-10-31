using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFPrototype.Toolkit.Controls
{
    /// <summary>
    /// 自定义List
    /// </summary>
    public class TKList : ItemsControl
    {
        #region constructors
        static TKList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TKList), new FrameworkPropertyMetadata(typeof(TKList)));
        }
        #endregion

        #region override methods
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TKListItem();
        }
        #endregion
    }
}
