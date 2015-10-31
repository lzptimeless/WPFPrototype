using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPFPrototype.Commons.Downloads
{
    public class FileSource
    {
        #region constructors
        public FileSource()
        { }

        public FileSource(string url)
        {
            this._url = url;
        }

        public FileSource(string url, string account, string password)
        {
            this._url = url;
            this._account = account;
            this._password = password;
            this._isNeedAuthentication = true;
        }
        #endregion

        #region properties
        #region Url
        private string _url;
        /// <summary>
        /// Get or set <see cref="Url"/>
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        #endregion

        #region Account
        private string _account;
        /// <summary>
        /// Get or set <see cref="Account"/>
        /// </summary>
        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }
        #endregion

        #region Password
        private string _password;
        /// <summary>
        /// Get or set <see cref="Password"/>
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        #endregion

        #region IsNeedAuthentication
        private bool _isNeedAuthentication;
        /// <summary>
        /// Get or set <see cref="IsNeedAuthentication"/>
        /// </summary>
        public bool IsNeedAuthentication
        {
            get { return _isNeedAuthentication; }
            set { _isNeedAuthentication = value; }
        }
        #endregion
        #endregion

        #region public methods
        public static FileSource CreateFromXElement(XElement element)
        {
            FileSource fileSrc = new FileSource();

            foreach (XElement field in element.Elements())
            {
                switch (field.Name.LocalName)
                {
                    case "Url":
                        fileSrc._url = field.Value;
                        break;
                    case "Account":
                        fileSrc._account = field.Value;
                        break;
                    case "Password":
                        fileSrc._password = field.Value;
                        break;
                    case "IsNeedAuthentication":
                        fileSrc._isNeedAuthentication = string.Equals("true", field.Value, StringComparison.InvariantCultureIgnoreCase) ? true : false;
                        break;
                    default:
                        break;
                }
            }// foreach

            return fileSrc;
        }

        public XElement ToXElement()
        {
            var element = new XElement("FileSource",
                    new XElement("Url", this._url ?? string.Empty),
                    new XElement("Account", this._account ?? string.Empty),
                    new XElement("Password", this._password ?? string.Empty),
                    new XElement("IsNeedAuthentication", this._isNeedAuthentication ? "true" : "false")
                );

            return element;
        }
        #endregion
    }
}
