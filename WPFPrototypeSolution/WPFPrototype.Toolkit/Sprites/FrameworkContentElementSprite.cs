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
    /// 动画精灵，依附于FrameworkContentElement执行和控制动画
    /// </summary>
    public class FrameworkContentElementSprite : ISprite
    {
        #region fields
        private FrameworkContentElement _element;
        private Storyboard _story;
        #endregion

        #region constructors
        private FrameworkContentElementSprite(FrameworkContentElement element)
        {
            this._element = element;

            element.Unloaded += Element_Unloaded;
        }
        #endregion

        #region properties
        #region Sprite
        public static FrameworkContentElementSprite GetSprite(FrameworkContentElement dObj)
        {
            return dObj.GetValue(FrameworkContentElementSprite.SpriteProperty) as FrameworkContentElementSprite;
        }

        public static void SetSprite(FrameworkContentElement dObj, FrameworkContentElementSprite sprite)
        {
            dObj.SetValue(FrameworkContentElementSprite.SpriteProperty, sprite);
        }

        // Using a DependencyProperty as the backing store for Sprite.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpriteProperty =
            DependencyProperty.RegisterAttached("Sprite", typeof(FrameworkContentElementSprite), typeof(FrameworkContentElementSprite), new PropertyMetadata(null));
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 从FrameworkContentElement，获取<see cref="FrameworkContentElementSprite"/>
        /// </summary>
        /// <param name="element">FrameworkContentElement</param>
        /// <returns></returns>
        public static FrameworkContentElementSprite FromElement(FrameworkContentElement element)
        {
            var sprite = FrameworkContentElementSprite.GetSprite(element);
            if (sprite == null)
            {
                sprite = new FrameworkContentElementSprite(element);
                FrameworkContentElementSprite.SetSprite(element, sprite);
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
