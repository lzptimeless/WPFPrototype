using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPrototype.Toolkit.Sprites;

namespace System.Windows
{
    public static class SpriteExtensions
    {
        /// <summary>
        /// 从FrameworkElement，获取<see cref="FrameworkElementSprite"/>
        /// </summary>
        /// <param name="element">FrameworkElement</param>
        /// <returns></returns>
        public static ISprite Sprite(this FrameworkElement element)
        {
            return FrameworkElementSprite.FromElement(element);
        }

        /// <summary>
        /// 从FrameworkContentElement，获取<see cref="FrameworkContentElementSprite"/>
        /// </summary>
        /// <param name="element">FrameworkContentElement</param>
        /// <returns></returns>
        public static ISprite Sprite(this FrameworkContentElement contentElement)
        {
            return FrameworkContentElementSprite.FromElement(contentElement);
        }
    }
}
