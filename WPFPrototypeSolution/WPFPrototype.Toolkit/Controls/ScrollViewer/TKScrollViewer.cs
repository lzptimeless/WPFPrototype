using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFPrototype.Toolkit.Controls
{
    public class TKScrollViewer : ScrollViewer
    {
        #region constrcutors

        #endregion

        #region override methods
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // ScrollViewer自带焦点导航无法移动到ScrollViewer外面，所以这里要屏蔽自带导航
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right) return;

            base.OnKeyDown(e);
        }
        #endregion
    }
}
