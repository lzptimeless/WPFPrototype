using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Toolkit.Controls
{
    public interface ITextCase
    {
        /// <summary>
        /// 获取或设置大小写
        /// </summary>
        TextCases TextCase { get; set; }

        /// <summary>
        /// 处理<see cref="TextCase"/>改变逻辑的方法
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void InvalidTestCase(TextCases oldValue, TextCases newValue);
    }
}
