using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFPrototype.Toolkit.Languages;

namespace WPFPrototype
{
    /// <summary>
    /// Interaction logic for LanguageWindow.xaml
    /// </summary>
    public partial class LanguageWindow : Window
    {
        public LanguageWindow()
        {
            InitializeComponent();

            this.Languages.ItemsSource = LanguageHelper.Resources.GetLanguages();
            this.Languages.SelectedValue = LanguageHelper.Resources.GetLanguage();
        }

        private void Languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded || this.Languages.SelectedValue == null) return;

            LanguageHelper.Resources.SetLanguage(this.Languages.SelectedValue.ToString());
        }
    }
}
