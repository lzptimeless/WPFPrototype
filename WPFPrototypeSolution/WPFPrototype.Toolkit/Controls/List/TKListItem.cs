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
    /// 自定义ListItem
    /// </summary>
    public class TKListItem : ContentControl
    {
        #region constructors
        static TKListItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TKListItem), new FrameworkPropertyMetadata(typeof(TKListItem)));
        }
        #endregion
    }
}
