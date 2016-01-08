using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 本地文件片段
    /// </summary>
    public class LocalSegment
    {
        #region properties
        #region Index
        private int _index;
        /// <summary>
        /// Get or set <see cref="Index"/>，片段索引
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        #endregion

        #region StartPosition
        private long _startPosition;
        /// <summary>
        /// Get or set <see cref="StartPosition"/>，片段起始位置
        /// </summary>
        public long StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }
        #endregion

        #region EndPosition
        private long _endPosition;
        /// <summary>
        /// Get or set <see cref="EndPosition"/>，片断结束位置
        /// </summary>
        public long EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }
        #endregion

        #region Position
        private long _position;
        /// <summary>
        /// Get or set <see cref="Position"/>，片段写入指针的位置
        /// </summary>
        public long Position
        {
            get { return _position; }
            set { _position = value; }
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 通过序列化数据创建<see cref="LocalSegment"/>
        /// </summary>
        /// <param name="element">序列化Xml节点</param>
        /// <returns></returns>
        public static LocalSegment CreateFromXElement(XElement element)
        {
            LocalSegment localSg = new LocalSegment();

            foreach (XElement field in element.Elements())
            {
                switch (field.Name.LocalName)
                {
                    case "Index":
                        localSg._index = int.Parse(field.Value);
                        break;
                    case "StartPosition":
                        localSg._startPosition = long.Parse(field.Value);
                        break;
                    case "EndPosition":
                        localSg._endPosition = long.Parse(field.Value);
                        break;
                    case "Position":
                        localSg._position = long.Parse(field.Value);
                        break;
                    default:
                        break;
                }
            }// foreach

            return localSg;
        }

        /// <summary>
        /// 序列化为Xml节点
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            XElement element = new XElement("LocalSegment",
                    new XElement("Index", this._index),
                    new XElement("StartPosition", this._startPosition),
                    new XElement("EndPosition", this._endPosition),
                    new XElement("Position", this._position)
                );

            return element;
        }
        #endregion
    }
}
