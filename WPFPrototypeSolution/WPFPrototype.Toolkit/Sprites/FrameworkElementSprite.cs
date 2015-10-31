using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPFPrototype.Toolkit.Sprites
{
    /// <summary>
    /// 动画精灵，依附于FrameworkElement执行和控制动画
    /// </summary>
    public class FrameworkElementSprite : ISprite
    {
        #region fields
        private FrameworkElement _element;
        private Storyboard _story;
        #endregion

        #region constructors
        private FrameworkElementSprite(FrameworkElement element)
        {
            this._element = element;

            element.Unloaded += Element_Unloaded;
        }
        #endregion

        #region properties
        #region Sprite
        public static FrameworkElementSprite GetSprite(FrameworkElement dObj)
        {
            return dObj.GetValue(FrameworkElementSprite.SpriteProperty) as FrameworkElementSprite;
        }

        public static void SetSprite(FrameworkElement dObj, FrameworkElementSprite sprite)
        {
            dObj.SetValue(FrameworkElementSprite.SpriteProperty, sprite);
        }

        // Using a DependencyProperty as the backing store for Sprite.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpriteProperty =
            DependencyProperty.RegisterAttached("Sprite", typeof(FrameworkElementSprite), typeof(FrameworkElementSprite), new PropertyMetadata(null));
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 从FrameworkElement，获取<see cref="FrameworkElementSprite"/>
        /// </summary>
        /// <param name="element">FrameworkElement</param>
        /// <returns></returns>
        public static FrameworkElementSprite FromElement(FrameworkElement element)
        {
            var sprite = FrameworkElementSprite.GetSprite(element);
            if (sprite == null)
            {
                sprite = new FrameworkElementSprite(element);
                FrameworkElementSprite.SetSprite(element, sprite);
            }

            return sprite;
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        /// <param name="story">Storyboard</param>
        public void Start(Storyboard story)
        {
            var oldStory = this._story;
            this._story = story;
            var element = this._element;

            story.Begin(element, HandoffBehavior.SnapshotAndReplace, true);

            if (oldStory != null) oldStory.Remove(element);
        }

        /// <summary>
        /// 暂停动画
        /// </summary>
        public void Pause()
        {
            var story = this._story;
            var element = this._element;

            if (story != null) story.Pause(element);
        }

        /// <summary>
        /// 恢复动画
        /// </summary>
        public void Resume()
        {
            var story = this._story;
            var element = this._element;

            if (story != null) story.Resume(element);
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void Stop()
        {
            this.ReleaseStoryboard();
        }
        #endregion

        #region event handlers
        void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ReleaseStoryboard();
        }
        #endregion

        #region private methods
        /// <summary>
        /// 释放Storyboard，受Storyboard影响的属性会自动复原
        /// </summary>
        private void ReleaseStoryboard()
        {
            var story = this._story;
            this._story = null;
            var element = this._element;

            if (story != null) story.Remove(element);
        }
        #endregion
    }
}
