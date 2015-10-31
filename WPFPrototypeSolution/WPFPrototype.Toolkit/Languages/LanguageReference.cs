using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Toolkit.Languages
{
    /// <summary>
    /// 语言引用，引用别的键值
    /// </summary>
    public class LanguageReference
    {
        #region constructors
        public LanguageReference(string key)
        {
            this._key = key;
        }
        #endregion

        #region properties
        #region Key
        private string _key;
        /// <summary>
        /// Get or set <see cref="Key"/>
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        #endregion
        #endregion
    }
}
