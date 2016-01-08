using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 下载文件信息
    /// </summary>
    public class RemoteFileInfo
    {
        #region properties
        #region MimeType
        private string _mimeType;
        /// <summary>
        /// Get or set <see cref="MimeType"/>，文件类型
        /// </summary>
        public string MimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }
        #endregion

        #region IsAcceptRange
        private bool _isAcceptRange;
        /// <summary>
        /// Get or set <see cref="IsAcceptRange"/>，是否支持分片
        /// </summary>
        public bool IsAcceptRange
        {
            get { return _isAcceptRange; }
            set { _isAcceptRange = value; }
        }
        #endregion

        #region Size
        private long _size;
        /// <summary>
        /// Get or set <see cref="Size"/>，大小
        /// </summary>
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        #endregion

        #region ModifyTime
        private DateTime _modifyTime;
        /// <summary>
        /// Get or set <see cref="ModifyTime"/>，最后修改时间
        /// </summary>
        public DateTime ModifyTime
        {
            get { return _modifyTime; }
            set { _modifyTime = value; }
        }
        #endregion

        #region MD5
        private string _md5;
        /// <summary>
        /// Get or set <see cref="MD5"/>
        /// </summary>
        public string MD5
        {
            get { return _md5; }
            set { _md5 = value; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 判断两个文件是否一样，优先用md5比较，如果没有md5则比较类型，大小
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsSameFile(RemoteFileInfo left, RemoteFileInfo right)
        {
            if ((!string.IsNullOrEmpty(left._md5)) &&
                (!string.IsNullOrEmpty(right._md5)) &&
                left._md5 == right._md5)
            {
                return true;
            }

            if (left._mimeType == right._mimeType &&
                left._size == right._size)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 通过序列化xml节点创建<see cref="RemoteFileInfo"/>
        /// </summary>
        /// <param name="element">序列化xml节点</param>
        /// <returns></returns>
        public static RemoteFileInfo CreateFromXElement(XElement element)
        {
            RemoteFileInfo remoteInfo = new RemoteFileInfo();

            foreach (XElement field in element.Elements())
            {
                switch (field.Name.LocalName)
                {
                    case "MimeType":
                        remoteInfo._mimeType = field.Value;
                        break;
                    case "IsAcceptRange":
                        remoteInfo._isAcceptRange = string.Equals("true", field.Value, StringComparison.InvariantCultureIgnoreCase) ? true : false;
                        break;
                    case "Size":
                        remoteInfo._size = long.Parse(field.Value);
                        break;
                    case "ModifyTime":
                        remoteInfo._modifyTime = DateTime.Parse(field.Value);
                        break;
                    case "MD5":
                        remoteInfo._md5 = field.Value;
                        break;
                    default:
                        break;
                }
            }// foreach

            return remoteInfo;
        }

        /// <summary>
        /// 序列化为xml节点
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            var element = new XElement("RemoteFileInfo",
                    new XElement("MimeType", this._mimeType ?? string.Empty),
                    new XElement("IsAcceptRange", this._isAcceptRange ? "true" : "false"),
                    new XElement("Size", this._size),
                    new XElement("ModifyTime", this._modifyTime.ToString("yyyy-MM-dd hh:mm:ss")),
                    new XElement("MD5", this._md5 ?? string.Empty)
                );

            return element;
        }
        #endregion
    }
}
