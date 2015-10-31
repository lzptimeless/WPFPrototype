using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace WPFPrototype.Toolkit.Sprites
{
    /// <summary>
    /// 动画精灵，依附于FrameworkElement或FrameworkContentElement执行和控制动画
    /// </summary>
    public interface ISprite
    { 
        /// <summary>
        /// 开始动画
        /// </summary>
        /// <param name="story"><see cref="Storyboard"/></param>
        void Start(Storyboard story);

        /// <summary>
        /// 暂停动画
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复动画
        /// </summary>
        void Resume();

        /// <summary>
        /// 停止动画
        /// </summary>
        void Stop();
    }
}
