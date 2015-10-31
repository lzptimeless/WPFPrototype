using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WPFPrototype.Toolkit.Controls
{
    /// <summary>
    /// 处理文本辅助类
    /// </summary>
    internal static class TextHelper
    {
        #region fields
        /// <summary>
        /// 用以提取格式化字符串中的片段
        /// </summary>
        private static readonly Regex ParameterRegex = new Regex(@"\{\s*(?<index>[0-9])\s*\}");
        #endregion

        #region public methods
        /// <summary>
        /// 分析格式化字符串
        /// </summary>
        /// <param name="formatString"></param>
        public static void AnalyzeFormatString(string formatString, AnalyzeFormatStringAction action)
        {
            if (string.IsNullOrEmpty(formatString)) return;

            var mhs = ParameterRegex.Matches(formatString); // 获取占位符
            int startIndex = 0;
            foreach (Match mh in mhs)
            {
                if (mh.Index > startIndex)
                {
                    // 处理占位符前的文本
                    string text = formatString.Substring(startIndex, mh.Index - startIndex);
                    action.Invoke(false, -1, text);
                }

                // 处理占位符
                int index = int.Parse(mh.Groups["index"].Value);
                action.Invoke(true, index, null);
                startIndex = mh.Index + mh.Length;
            }// foreach

            // 处理末尾的文本
            if (formatString.Length > startIndex)
            {
                string text = formatString.Substring(startIndex, formatString.Length - startIndex);
                action.Invoke(false, -1, text);
            }// if
        }

        /// <summary>
        /// 设置文本大小写
        /// </summary>
        /// <param name="s">原始文本</param>
        /// <param name="textCase">大小写</param>
        /// <returns></returns>
        public static string ApplyTextCase(string s, TextCases textCase)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            if (textCase == TextCases.Orginal) return s; // 使用原始大小写

            if (textCase == TextCases.Upper) return s.ToUpperInvariant(); // 使用全大写

            if (textCase == TextCases.Lower) return s.ToLowerInvariant(); // 使用全小写

            if (textCase == TextCases.Title) return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s); // 单词首字母大写

            return s;
        }
        #endregion
    }

    /// <summary>
    /// 分析格式化字符串的委托
    /// </summary>
    /// <param name="isParameter">当前部分是否是格式化参数</param>
    /// <param name="parameterIndex">如果当前部分是格式化参数，则代表格式化参数索引，不是的话这个值没有意义</param>
    /// <param name="text">如果当前部分不是格式化参数，这个值代表文本，如果是格式化参数则这个值没有意义</param>
    public delegate void AnalyzeFormatStringAction(bool isParameter, int parameterIndex, string text);
}
