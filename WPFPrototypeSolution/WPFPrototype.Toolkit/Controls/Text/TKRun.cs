using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using WPFPrototype.Toolkit.Languages;

namespace WPFPrototype.Toolkit.Controls
{
    /// <summary>
    /// 支持格式化字符串的Run
    /// </summary>
    public class TKRun : Run, ITextCase
    {
        #region constructors
        static TKRun()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TKRun), new FrameworkPropertyMetadata(typeof(TKRun)));
        }

        public TKRun()
        {
            this.Loaded += TKRun_Loaded;
            this.Unloaded += TKRun_Unloaded;
        }
        #endregion

        #region properties
        #region TextKey
        /// <summary>
        /// 多语言键值
        /// </summary>
        public string TextKey
        {
            get { return GetValue(TextKeyProperty) as string; }
            set { SetValue(TextKeyProperty, value); }
        }

        /// <summary>
        /// The <see cref="TextKey"/> property
        /// </summary>
        public static readonly DependencyProperty TextKeyProperty =
            DependencyProperty.Register("TextKey", typeof(string), typeof(TKRun), new FrameworkPropertyMetadata(null, TextKeyChanged));

        private static void TextKeyChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnTextKeyChanged(e.OldValue as string, e.NewValue as string);
        }

        protected virtual void OnTextKeyChanged(string oldValue, string newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter0
        /// <summary>
        /// 格式化文本参数0
        /// </summary>
        public object Parameter0
        {
            get { return GetValue(Parameter0Property) as object; }
            set { SetValue(Parameter0Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter0"/>
        /// </summary>
        public static readonly DependencyProperty Parameter0Property =
            DependencyProperty.Register("Parameter0", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter0Changed));

        private static void Parameter0Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter0Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter0Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter1
        /// <summary>
        /// 格式化文本参数1
        /// </summary>
        public object Parameter1
        {
            get { return GetValue(Parameter1Property) as object; }
            set { SetValue(Parameter1Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter1"/>
        /// </summary>
        public static readonly DependencyProperty Parameter1Property =
            DependencyProperty.Register("Parameter1", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter1Changed));

        private static void Parameter1Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter1Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter1Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter2
        /// <summary>
        /// 格式化文本参数2
        /// </summary>
        public object Parameter2
        {
            get { return GetValue(Parameter2Property) as object; }
            set { SetValue(Parameter2Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter2"/>
        /// </summary>
        public static readonly DependencyProperty Parameter2Property =
            DependencyProperty.Register("Parameter2", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter2Changed));

        private static void Parameter2Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter2Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter2Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter3
        /// <summary>
        /// 格式化文本参数3
        /// </summary>
        public object Parameter3
        {
            get { return GetValue(Parameter3Property) as object; }
            set { SetValue(Parameter3Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter3"/>
        /// </summary>
        public static readonly DependencyProperty Parameter3Property =
            DependencyProperty.Register("Parameter3", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter3Changed));

        private static void Parameter3Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter3Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter3Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter4
        /// <summary>
        /// 格式化文本参数4
        /// </summary>
        public object Parameter4
        {
            get { return GetValue(Parameter4Property) as object; }
            set { SetValue(Parameter4Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter4"/>
        /// </summary>
        public static readonly DependencyProperty Parameter4Property =
            DependencyProperty.Register("Parameter4", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter4Changed));

        private static void Parameter4Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter4Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter4Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter5
        /// <summary>
        /// 格式化文本参数5
        /// </summary>
        public object Parameter5
        {
            get { return GetValue(Parameter5Property) as object; }
            set { SetValue(Parameter5Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter5"/>
        /// </summary>
        public static readonly DependencyProperty Parameter5Property =
            DependencyProperty.Register("Parameter5", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter5Changed));

        private static void Parameter5Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter5Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter5Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter6
        /// <summary>
        /// 格式化文本参数6
        /// </summary>
        public object Parameter6
        {
            get { return GetValue(Parameter6Property) as object; }
            set { SetValue(Parameter6Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter6"/>
        /// </summary>
        public static readonly DependencyProperty Parameter6Property =
            DependencyProperty.Register("Parameter6", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter6Changed));

        private static void Parameter6Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter6Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter6Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter7
        /// <summary>
        /// 格式化文本参数7
        /// </summary>
        public object Parameter7
        {
            get { return GetValue(Parameter7Property) as object; }
            set { SetValue(Parameter7Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter7"/>
        /// </summary>
        public static readonly DependencyProperty Parameter7Property =
            DependencyProperty.Register("Parameter7", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter7Changed));

        private static void Parameter7Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter7Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter7Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter8
        /// <summary>
        /// 格式化文本参数8
        /// </summary>
        public object Parameter8
        {
            get { return GetValue(Parameter8Property) as object; }
            set { SetValue(Parameter8Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter8"/>
        /// </summary>
        public static readonly DependencyProperty Parameter8Property =
            DependencyProperty.Register("Parameter8", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter8Changed));

        private static void Parameter8Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter8Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter8Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region Parameter9
        /// <summary>
        /// 格式化文本参数9
        /// </summary>
        public object Parameter9
        {
            get { return GetValue(Parameter9Property) as object; }
            set { SetValue(Parameter9Property, value); }
        }

        /// <summary>
        /// The property of <see cref="Parameter9"/>
        /// </summary>
        public static readonly DependencyProperty Parameter9Property =
            DependencyProperty.Register("Parameter9", typeof(object), typeof(TKRun), new FrameworkPropertyMetadata(null, Parameter9Changed));

        private static void Parameter9Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnParameter9Changed(e.OldValue, e.NewValue);
        }

        protected virtual void OnParameter9Changed(object oldValue, object newValue)
        {
            RefreshText();
        }
        #endregion

        #region OriginalText
        /// <summary>
        /// 原始文本，除了通过<see cref="TextKey"/>设置文本以外，通过这个属性设置文本也根据<see cref="TextCase"/>处理大小写
        /// </summary>
        public string OriginalText
        {
            get { return GetValue(OriginalTextProperty) as string; }
            set { SetValue(OriginalTextProperty, value); }
        }

        public static readonly DependencyProperty OriginalTextProperty =
            DependencyProperty.Register("OriginalText", typeof(string), typeof(TKRun), new FrameworkPropertyMetadata(null, OriginalTextChanged));

        private static void OriginalTextChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            ((TKRun)dObj).OnOriginalTextChanged(e.OldValue as string, e.NewValue as string);
        }

        protected virtual void OnOriginalTextChanged(string oldValue, string newValue)
        {
            RefreshText();
        }
        #endregion

        #region TextCase
        public static TextCases GetTextCase(DependencyObject dObj)
        {
            return (TextCases)dObj.GetValue(TextCaseProperty);
        }

        public static void SetTextCase(DependencyObject dObj, TextCases value)
        {
            dObj.SetValue(TextCaseProperty, value);
        }

        /// <summary>
        /// 获取或设置文本大小写类型
        /// </summary>
        public TextCases TextCase
        {
            get { return (TextCases)GetValue(TextCaseProperty); }
            set { SetValue(TextCaseProperty, value); }
        }

        public static readonly DependencyProperty TextCaseProperty =
            DependencyProperty.RegisterAttached("TextCase", typeof(TextCases), typeof(TKRun), new FrameworkPropertyMetadata(TextCases.Orginal, FrameworkPropertyMetadataOptions.Inherits, TextCaseChanged));

        private static void TextCaseChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            if (dObj is ITextCase)
                ((ITextCase)dObj).InvalidTestCase((TextCases)e.OldValue, (TextCases)e.NewValue);
        }

        public virtual void InvalidTestCase(TextCases oldValue, TextCases newValue)
        {
            RefreshText();
        }
        #endregion
        #endregion

        #region event handlers
        void Resources_LanguageChanged(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                // 为了解决Converter不能更新的问题，更新绑定数据
                var dps = new[] { 
                Parameter0Property,
                Parameter1Property,
                Parameter2Property,
                Parameter3Property,
                Parameter4Property,
                Parameter5Property,
                Parameter6Property,
                Parameter7Property,
                Parameter8Property,
                Parameter9Property,
                OriginalTextProperty
                };
                foreach (var dp in dps)
                {
                    var be = BindingOperations.GetBindingExpression(this, dp);
                    if (be != null) be.UpdateTarget();
                }

                this.RefreshText();
            });
        }

        private void TKRun_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshText();
            LanguageHelper.Resources.AddWeakLanguageChangedHandler(Resources_LanguageChanged);
        }

        private void TKRun_Unloaded(object sender, RoutedEventArgs e)
        {
            LanguageHelper.Resources.RemoveWeakLanguageChangedHandler(Resources_LanguageChanged);
        }
        #endregion

        #region private methods
        private void RefreshText()
        {
            // 为了增加效率，Loaded之后才刷新，而且这样才能避免初始化显示时大小写失效的BUG
            if (!this.IsLoaded) return;

            string originalText = this.OriginalText;
            TextCases textCase = this.TextCase;
            if (!string.IsNullOrEmpty(originalText))
            {
                this.Text = TextHelper.ApplyTextCase(originalText, textCase);
                return;
            }

            object[] parameters = new[] { 
                Parameter0,
                Parameter1,
                Parameter2,
                Parameter3,
                Parameter4,
                Parameter5,
                Parameter6,
                Parameter7,
                Parameter8,
                Parameter9
            };
            string formatText = LanguageHelper.GetLanguage(this.TextKey);
            if (string.IsNullOrEmpty(formatText))
            {
                this.Text = null;
                return;
            }

            StringBuilder sb = new StringBuilder();
            TextHelper.AnalyzeFormatString(formatText, (isParam, paramIndex, text) =>
            {
                if (isParam)
                {
                    string param = null;
                    if (parameters.Length > paramIndex && parameters[paramIndex] != null)
                        param = parameters[paramIndex].ToString();
                    else
                        param = string.Format("{{{0}}}", paramIndex);

                    sb.Append(param);
                }
                else
                {
                    sb.Append(text);
                }
            });

            this.Text = TextHelper.ApplyTextCase(sb.ToString(), textCase);
        }
        #endregion
    }
}
