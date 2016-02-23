using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WPFPrototype.Commons.Downloads;

namespace WPFPrototype
{
    /// <summary>
    /// Interaction logic for DownloadManagerWindow.xaml
    /// </summary>
    public partial class DownloadManagerWindow : Window
    {
        #region fields
        private const string Url = "http://cdn.akamai.steamstatic.com/steam/apps/389870/capsule_616x353.jpg?t=1445501727";
        private string _savePath;
        #endregion

        public DownloadManagerWindow()
        {
            InitializeComponent();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            _savePath = @"c:\" + DownloadHelper.GetFileNameFromUrl(Url);
            Downloader downloader = new Downloader(Url, _savePath,2);
            downloader.Completed += Downloader_Completed;
            downloader.Start();
        }

        private void Downloader_Completed(object sender, EventArgs e)
        {
            Process.Start(this._savePath);
        }
    }
}
