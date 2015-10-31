using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFPrototype.Toolkit.Controls
{
    /// <summary>
    /// 自定义ComboBox
    /// </summary>
    public class TKComboBox : ComboBox
    {
        #region constrcutors
        static TKComboBox()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TKComboBox), new FrameworkPropertyMetadata(typeof(TKComboBox)));
        }
        #endregion

        #region override methods
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TKComboBoxItem();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // ComboBox中的Up，Down本来是用来选择的，但我们希望它是焦点移动，所以要忽略Up，Down这两个按键
            // 当IsDropDownOpen=true时Up，Down被当作焦点移动和记录选项的功能，所以在IsDropDownOpen=true
            // 时需要使用默认操作
            if ((e.Key == Key.Up || e.Key == Key.Down) && !this.IsDropDownOpen) return;

            // Left，Right原本也是用来选择的，但我们希望它是焦点移动，所以要忽略
            if (e.Key == Key.Left || e.Key == Key.Right) return;

            // ComboBox本来无法通过键盘打开列表，这里添加这个功能
            if (e.Key == Key.Enter && !this.IsDropDownOpen)
            {
                this.IsDropDownOpen = true;
                return;
            }

            base.OnKeyDown(e);
        }
        #endregion
    }
}
