using System;
using System.Collections.Generic;
using System.IO;
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
using WPFPrototype.Toolkit.Gifs;

namespace WPFPrototype
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        private GifPlayer _gifPlayer;

        public ImageWindow()
        {
            InitializeComponent();

            this._gifPlayer = new GifPlayer();
            this._gifPlayer.ImageSourceChangedEvent += _gifPlayer_ImageSourceChangedEvent;
        }

        void _gifPlayer_ImageSourceChangedEvent(object sender, EventArgs e)
        {
            this.ImageHost.Source = this._gifPlayer.ImageSource;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            this._gifPlayer.Pause();
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            this._gifPlayer.Resume();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this._gifPlayer.Stop();
        }

        private void SetSpeed_Click(object sender, RoutedEventArgs e)
        {
            double speed;
            if (double.TryParse(this.Speed.Text, out speed))
                this._gifPlayer.SetSpeed(speed);
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;
                var files = Directory.GetFiles(path, "*.gif");
                this.Images.ItemsSource = files;
            }
        }

        private void Images_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var path = this.Images.SelectedValue as string;
            if (!string.IsNullOrEmpty(path))
                this._gifPlayer.Play(path);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ToolHost.Visibility = ToolHost.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
