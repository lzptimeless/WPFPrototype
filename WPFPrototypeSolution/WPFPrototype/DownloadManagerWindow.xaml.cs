using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private const string Url = "http://down.sandai.net/thunderspeed/ThunderSpeed1.0.33.358.exe";
        private string _savePath;
        private Downloader _downloader;
        private DateTime _startTime;
        #endregion

        public DownloadManagerWindow()
        {
            InitializeComponent();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            _savePath = System.IO.Path.Combine(@"C:\Temporary\Other", DownloadHelper.GetFileNameFromUrl(Url));

            this._downloader = new Downloader(Url, _savePath, 4, 0);
            this._downloader.Completed += Downloader_Completed;
            this._downloader.ProgressChanged += Downloader_ProgressChanged;
            this._downloader.Paused += Downloader_Paused;
            this._downloader.Cancelled += Downloader_Cancelled;
            this._downloader.Failed += Downloader_Failed;
            this._downloader.Start();
            this._startTime = DateTime.Now;
        }

        private void Downloader_Failed(object sender, EventArgs e)
        {
            MessageBox.Show("Failed");
        }

        private void Downloader_Cancelled(object sender, EventArgs e)
        {
            MessageBox.Show("Cancelled");
        }

        private void Downloader_Paused(object sender, EventArgs e)
        {
            MessageBox.Show("Paused");
        }

        private void Downloader_ProgressChanged(object sender, DownloadProgressArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Progress.Value = e.Progress;
                this.Speed.Text = string.Format("{0}KB/S", e.Speed / 1024);
                //this.Segments.ItemsSource = e.Segments.Select(p => new SegmentViewModel(p));
            }));
        }

        private void Downloader_Completed(object sender, EventArgs e)
        {
            Process.Start(this._savePath);
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Speed.Text = string.Format("Take Time:{0}", DateTime.Now - this._startTime);
            }));
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            this._downloader.Pause();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this._downloader.Cancel();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            this._downloader.Start();
        }
    }

    public class SegmentViewModel
    {
        #region fields

        #endregion

        #region constructors
        public SegmentViewModel(LocalSegment segment)
        {
            this._startPosition = segment.StartPosition;
            this._endPosition = segment.EndPosition;
            this._position = segment.Position;
            this._length = segment.EndPosition - segment.StartPosition + 1;
            this._progress = (int)(this._position * 100 / this._length);
        }
        #endregion

        #region properties
        #region StartPosition
        private long _startPosition;
        /// <summary>
        /// Get or set <see cref="StartPosition"/>
        /// </summary>
        public long StartPosition
        {
            get { return this._startPosition; }
            set { this._startPosition = value; }
        }
        #endregion

        #region EndPosition
        private long _endPosition;
        /// <summary>
        /// Get or set <see cref="EndPosition"/>
        /// </summary>
        public long EndPosition
        {
            get { return this._endPosition; }
            set { this._endPosition = value; }
        }
        #endregion

        #region Position
        private long _position;
        /// <summary>
        /// Get or set <see cref="Position"/>
        /// </summary>
        public long Position
        {
            get { return this._position; }
            set { this._position = value; }
        }
        #endregion

        #region Length
        private long _length;
        /// <summary>
        /// Get or set <see cref="Length"/>
        /// </summary>
        public long Length
        {
            get { return this._length; }
            set { this._length = value; }
        }
        #endregion

        #region Progress
        private int _progress;
        /// <summary>
        /// Get or set <see cref="Progress"/>
        /// </summary>
        public int Progress
        {
            get { return this._progress; }
            set { this._progress = value; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion
    }
}
