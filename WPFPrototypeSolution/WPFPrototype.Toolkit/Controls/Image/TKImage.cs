using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFPrototype.Toolkit.Gifs;

namespace WPFPrototype.Toolkit.Controls
{
    /// <summary>
    /// 图像空间，支持Gif动画
    /// </summary>
    [TemplatePart(Name = PART_Image, Type = typeof(Image))]
    public class TKImage : Control
    {
        #region fields
        public const string PART_Image = "PART_Image";

        private Image _part_Image;
        private GifPlayer _gifPlayer;
        #endregion

        #region constructors
        public TKImage()
        {
            this.Loaded += TKImage_Loaded;
            this.Unloaded += TKImage_Unloaded;
        }
        #endregion

        #region properties
        #region Source
        /// <summary>
        /// Get or set <see cref="Source"/>，支持string, Uri, ImageSource
        /// </summary>
        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> of <see cref="Source"/>
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(TKImage), new PropertyMetadata(null, SourceChanged));

        private static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((TKImage)obj).OnSourceChanged((object)e.NewValue, (object)e.OldValue);
        }

        protected virtual void OnSourceChanged(object newValue, object oldValue)
        {
            if (newValue == null)
            {
                if (this._part_Image != null) this._part_Image.Source = null;
            }
            else if (newValue is string)
            { }
            else if (newValue is Uri)
            { }
            else if (newValue is ImageSource)
            { }
            else
            {
                // 不支持这种类型的图像源
                throw new NotSupportedException(string.Format("Not support {0} source.", newValue.GetType().FullName));
            }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._part_Image = this.GetTemplateChild(PART_Image) as Image;
        }

        private void TKImage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this._gifPlayer != null)
            {
                this._gifPlayer.Dispose();
                this._gifPlayer = null;
            }
        }

        private void TKImage_Loaded(object sender, RoutedEventArgs e)
        {
        }
        #endregion
    }
}
