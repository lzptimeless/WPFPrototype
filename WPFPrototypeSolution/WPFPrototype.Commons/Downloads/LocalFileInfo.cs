using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPFPrototype.Commons.Downloads
{
    public class LocalFileInfo
    {
        #region properties
        #region Source
        private FileSource _source;
        /// <summary>
        /// Get or set <see cref="Source"/>
        /// </summary>
        public FileSource Source
        {
            get { return _source; }
            set { _source = value; }
        }
        #endregion

        #region Mirrors
        private List<FileSource> _mirrors;
        /// <summary>
        /// Get or set <see cref="Mirrors"/>
        /// </summary>
        public List<FileSource> Mirrors
        {
            get { return _mirrors; }
            set { _mirrors = value; }
        }
        #endregion

        #region SavePath
        private string _savePath;
        /// <summary>
        /// Get or set <see cref="SavePath"/>
        /// </summary>
        public string SavePath
        {
            get { return _savePath; }
            set { _savePath = value; }
        }
        #endregion

        #region RemoteInfo
        private RemoteFileInfo _remoteInfo;
        /// <summary>
        /// Get or set <see cref="RemoteInfo"/>
        /// </summary>
        public RemoteFileInfo RemoteInfo
        {
            get { return _remoteInfo; }
            set { _remoteInfo = value; }
        }
        #endregion

        #region Segments
        private List<LocalSegment> _segments;
        /// <summary>
        /// Get or set <see cref="Segments"/>
        /// </summary>
        public List<LocalSegment> Segments
        {
            get { return _segments; }
            set { _segments = value; }
        }
        #endregion

        #region CreateTime
        private DateTime _createTime;
        /// <summary>
        /// Get or set <see cref="CreateTime"/>
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        #endregion

        #region HasSegment
        /// <summary>
        /// Get or set <see cref="HasSegment"/>
        /// </summary>
        public bool HasSegment
        {
            get { return this._segments != null && this._segments.Count > 0; }
        }
        #endregion
        #endregion

        #region public methods
        public static LocalFileInfo Load(string path)
        {
            XDocument xml = XDocument.Load(path);
            LocalFileInfo fileInfo = new LocalFileInfo();

            foreach (XElement field in xml.Root.Elements())
            {
                switch (field.Name.LocalName)
                {
                    case "Source":
                        {
                            if (string.IsNullOrEmpty(field.Value)) break;

                            fileInfo._source = FileSource.CreateFromXElement(field);
                        }
                        break;
                    case "Mirrors":
                        {
                            List<FileSource> mirrors = new List<FileSource>();
                            foreach (var mirror in field.Elements())
                            {
                                mirrors.Add(FileSource.CreateFromXElement(mirror));
                            }

                            fileInfo._mirrors = mirrors;
                        }
                        break;
                    case "SavePath":
                        fileInfo._savePath = field.Value ?? string.Empty;
                        break;
                    case "RemoteInfo":
                        {
                            if (string.IsNullOrEmpty(field.Value)) break;

                            fileInfo._remoteInfo = RemoteFileInfo.CreateFromXElement(field);
                        }
                        break;
                    case "Segments":
                        {
                            List<LocalSegment> segments = new List<LocalSegment>();
                            foreach (var segment in field.Elements())
                            {
                                segments.Add(LocalSegment.CreateFromXElement(segment));
                            }

                            fileInfo._segments = segments;
                        }
                        break;
                    case "CreateTime":
                        fileInfo._createTime = DateTime.Parse(field.Value);
                        break;
                    default:
                        break;
                }
            }// foreach

            return fileInfo;
        }

        public void Save(string path)
        {
            XDocument xml = new XDocument(new XElement("LocalFileInfo"));
            XElement root = xml.Root;

            if (this._source != null) root.Add(this._source.ToXElement());
            if (this._mirrors != null && this._mirrors.Count > 0)
            {
                XElement mirrors = new XElement("Mirrors");
                foreach (var mirror in this._mirrors)
                {
                    mirrors.Add(mirror.ToXElement());
                }
                root.Add(mirrors);
            }// if
            if (!string.IsNullOrEmpty(this._savePath)) root.Add(new XElement("SavePath", this._savePath));
            if (this._remoteInfo != null) root.Add(this._remoteInfo.ToXElement());

            if (this._segments != null && this._segments.Count > 0)
            {
                XElement segments = new XElement("Segments");
                foreach (var segment in this._segments)
                {
                    segments.Add(segment.ToXElement());
                }
                root.Add(segments);
            }// if
            root.Add(new XElement("CreateTime", this._createTime.ToString("yyyy-MM-dd hh:mm:ss")));

            xml.Save(path);
        }
        #endregion
    }
}
