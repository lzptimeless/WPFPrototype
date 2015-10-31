using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Toolkit.Gifs
{
    internal class GifFrame
    {
        #region fields
        public int Top;
        public int Left;
        public int PixelWidth;
        public int PixelHeight;
        public int[] Colors;
        public int Delay;
        public int Stride;
        #endregion
    }
}
