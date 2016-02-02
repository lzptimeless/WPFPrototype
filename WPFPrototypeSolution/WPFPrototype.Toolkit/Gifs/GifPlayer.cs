using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPFPrototype.Toolkit.Gifs
{
    /// <summary>
    /// Gif播放器
    /// </summary>
    public class GifPlayer : IDisposable
    {
        #region fields
        /// <summary>
        /// 用于这个对象内部lock
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// 用以显示的图像源
        /// </summary>
        private WriteableBitmap _bitmap;
        /// <summary>
        /// Gif缓存
        /// </summary>
        private MemoryStream _gifStream;
        /// <summary>
        /// Gif解码器
        /// </summary>
        private BitmapDecoder _decoder;
        /// <summary>
        /// Gif帧信息缓存，主要是为了节省解析这些信息的cpu消耗
        /// </summary>
        private GifFrame[] _frames;
        /// <summary>
        /// 切换帧计时器
        /// </summary>
        private DispatcherTimer _timer;
        /// <summary>
        /// 下一帧索引
        /// </summary>
        private int _nextIndex;
        /// <summary>
        /// 动画播放速度，默认1
        /// </summary>
        private double _speed;
        #endregion

        #region constructors
        public GifPlayer()
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// 图像源
        /// </summary>
        public ImageSource ImageSource
        {
            get { return this._bitmap; }
        }
        #endregion

        #region events
        #region ImageSourceChanged
        /// <summary>
        /// Event name of <see cref="ImageSourceChanged"/>
        /// </summary>
        public const string ImageSourceChangedEventName = "ImageSourceChanged";

        public event EventHandler<EventArgs> ImageSourceChanged;

        private void OnImageSourceChanged()
        {
            EventHandler<EventArgs> handler;

            lock (this._syncRoot)
            {
                handler = this.ImageSourceChanged;
            }

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakImageSourceChangedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<GifPlayer, EventArgs>.AddHandler(this, ImageSourceChangedEventName, handler);
        }

        public void RemoveWeakImageSourceChangedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<GifPlayer, EventArgs>.RemoveHandler(this, ImageSourceChangedEventName, handler);
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 播放Gif
        /// </summary>
        /// <param name="filePath">Gif文件路径</param>
        public async Task Play(string filePath)
        {
            // 终止当前动画，释放资源
            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer = null;
                this._frames = null;
                if (this._gifStream != null)
                {
                    this._gifStream.Dispose();
                    this._gifStream = null;
                }
                this._decoder = null;
            }

            MemoryStream gifStream = new MemoryStream(); // gif流缓存
            this._gifStream = gifStream;
            // 加载gif流，读取硬盘可能比较慢，这里用异步操作
            try
            {
                await Task.Run(() =>
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        gifStream.Capacity = (int)fs.Length;
                        fs.CopyTo(gifStream);
                        gifStream.Seek(0, SeekOrigin.Begin);
                    }
                });
            }
            catch
            {
                gifStream.Dispose();
                throw;
            }// catch

            // 如果当前操作被覆盖了则直接退出
            if (!object.ReferenceEquals(gifStream, this._gifStream))
            {
                gifStream.Dispose();
                return;
            }

            // 初始化gif解码器
            var decoder = BitmapDecoder.Create(gifStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            this._decoder = decoder;
            // 解析gif基础信息
            var logscrdesc = decoder.Metadata.GetQuery("/logscrdesc") as BitmapMetadata;
            int pixelWidth = (ushort)logscrdesc.GetQuery("/width");
            int pixelHeight = (ushort)logscrdesc.GetQuery("/height");
            // 初始化gif帧信息缓存
            this._frames = new GifFrame[decoder.Frames.Count];
            // 创建图像源
            var bitmap = this._bitmap = new WriteableBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null);
            this.OnImageSourceChanged();

            // 准备好下一帧
            this._nextIndex = 0;

            // 开始动画计时器
            this._speed = 1;
            this._timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Render, this.TimerCallback, Dispatcher.CurrentDispatcher);
            this._timer.Start();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            var timer = this._timer;
            if (timer != null) timer.Stop();
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Resume()
        {
            var timer = this._timer;
            if (timer != null) timer.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                // 渲染画面到第0帧
                this._nextIndex = 0;
                this.RenderNextFrame();
            }
        }

        /// <summary>
        /// 释放gif缓存，计时器
        /// </summary>
        public void Dispose()
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer = null;
                this._frames = null;
                if (this._gifStream != null)
                {
                    this._gifStream.Dispose();
                    this._gifStream = null;
                }
                this._decoder = null;

                this._bitmap = null;
                this.OnImageSourceChanged();
            }
        }

        /// <summary>
        /// 设置动画播放速度
        /// </summary>
        public void SetSpeed(double speed)
        {
            if (speed <= 0) throw new ArgumentOutOfRangeException("speed", "[speed] must bigger than 0");

            this._speed = speed;
        }
        #endregion

        #region private methods
        private void TimerCallback(object sender, EventArgs e)
        {
            var timer = this._timer;
            // 为了防止gif更新了之后，老的timer再次被渲染，所以发现timer作废后停止
            if (!object.ReferenceEquals(timer, sender)) return;

            timer.Stop();
            this.RenderNextFrame();

            // 开始下一帧计时
            int index = ++this._nextIndex;
            if (this._frames.Length <= index)
            {
                index = this._nextIndex = 0;
            }

            var gifFrame = this.GetOrCreateGifFrame(index);
            timer.Interval = TimeSpan.FromMilliseconds(gifFrame.Delay / this._speed);
            timer.Start();
        }

        /// <summary>
        /// 切换当前图像到下一帧
        /// </summary>
        private void RenderNextFrame()
        {
            // 由于gif最大是256色，而且使用的是调色板模式，像素大小为1byte，里面存的都是颜色索引，所以以下算法都是基于这个前提编写的
            int frameIndex = this._nextIndex;
            var bitmap = this._bitmap;
            var bitmapFrame = this._decoder.Frames[frameIndex];
            var frame = this.GetOrCreateGifFrame(frameIndex);
            int bitmapStride = bitmap.BackBufferStride;
            int frameTop = frame.Top;
            int frameLeft = frame.Left;
            int frameStride = frame.Stride;
            int[] colors = frame.Colors;
            byte[] pixels = new byte[frameStride * frame.PixelHeight];

            // 获取像素
            bitmapFrame.CopyPixels(pixels, frameStride, 0);

            // 锁定BackBuffer开始渲染下一帧
            bitmap.Lock();
            unsafe
            {
                int pBackBufferHeader = (int)bitmap.BackBuffer;
                int pBackBuffer = pBackBufferHeader;
                int start = 0, end = 0, r = 0, i = 0, framePixel;
                for (r = 0; r < frame.PixelHeight; r++)
                {
                    // 计算开始写入的BackBuffer的位置
                    pBackBuffer = pBackBufferHeader;
                    pBackBuffer += (r + frameTop) * bitmapStride;
                    pBackBuffer += frameLeft * 4; // 4代表4个字节，因为bitmap的格式为像素Bgra32
                    start = r * frameStride;
                    end = (r + 1) * frameStride;
                    // 开始写入frame第i行像素
                    for (i = start; i < end; i++, pBackBuffer += 4)
                    {
                        framePixel = colors[pixels[i]];
                        // 如果像素是透明的则忽略这个像素
                        if (framePixel == 0)
                        {
                            continue;
                        }

                        *((int*)pBackBuffer) = framePixel;
                    }// for
                }// for
            }

            // 设置刷新区域
            Int32Rect rect = new Int32Rect
            {
                X = frame.Left,
                Y = frame.Top,
                Width = Math.Min(frame.PixelWidth, bitmap.PixelWidth - frame.Left),
                Height = Math.Min(frame.PixelHeight, bitmap.PixelHeight - frame.Top)
            };
            bitmap.AddDirtyRect(rect);
            // 解除BackBuffer锁定
            bitmap.Unlock();
        }

        /// <summary>
        /// 获取或者创建<see cref="GifFrame"/>
        /// </summary>
        /// <param name="index">帧索引</param>
        private GifFrame GetOrCreateGifFrame(int index)
        {
            var frame = this._frames[index];

            // 如果没有解析过这一帧则解析它，并缓存下来
            if (frame == null)
            {
                var bitmapFrame = this._decoder.Frames[index];
                frame = CreateGifFrame(bitmapFrame);
                this._frames[index] = frame;
            }// if

            return frame;
        }

        /// <summary>
        /// 创建<see cref="GifFrame"/>
        /// </summary>
        /// <param name="frame">Gif解码的帧</param>
        /// <returns></returns>
        private static GifFrame CreateGifFrame(BitmapFrame frame)
        {
            GifFrame gifFrame = new GifFrame();

            var metadata = frame.Metadata as BitmapMetadata;
            var imgDesc = metadata.GetQuery("/imgdesc") as BitmapMetadata;// 帧描述
            gifFrame.Top = (ushort)imgDesc.GetQuery("/top"); // 当前帧在画布中的top坐标
            gifFrame.Left = (ushort)imgDesc.GetQuery("/left");// 当前帧在画布中的left坐标
            var grctlext = metadata.GetQuery("/grctlext") as BitmapMetadata;
            int delay = (ushort)grctlext.GetQuery("/delay") * 10;// 当前帧相对于前一帧的播放延迟时间，单位/10 ms
            gifFrame.Delay = delay > 10 ? delay : 100; // 有可能遇到"/delay"为1的情况，貌似这种情况默认为100 ms
            bool transparencyFlag = (bool)grctlext.GetQuery("/TransparencyFlag");// 当前帧是否支持透明色
            gifFrame.PixelWidth = frame.PixelWidth;// 当前帧宽度px
            gifFrame.PixelHeight = frame.PixelHeight;// 当前帧高度px
            // 当调色板中的颜色导出成int型的数组，为渲染操作节省运算量
            gifFrame.Colors = new int[frame.Palette.Colors.Count];
            int i = 0, color = 0;
            foreach (var item in frame.Palette.Colors)
            {
                if (transparencyFlag && item.A == 0)
                {
                    color = 0;
                }
                else
                {
                    color = item.A << 24;
                    color |= item.R << 16;
                    color |= item.G << 8;
                    color |= item.B << 0;
                }
                gifFrame.Colors[i] = color;
                ++i;
            }// foreach
            // 获取stride
            gifFrame.Stride = GetFrameStride(frame);

            return gifFrame;
        }

        private static int GetFrameStride(BitmapFrame frame)
        {
            int mask = (frame.Format.BitsPerPixel - 1);

            return (((frame.PixelWidth * frame.Format.BitsPerPixel + mask) & ~mask) / 8);
        }
        #endregion
    }
}
