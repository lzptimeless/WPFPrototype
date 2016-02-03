using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 按先后顺序依次选择镜像，原始数据源优先
    /// </summary>
    public class SequentialMirrorSelector : IMirrorSelector
    {
        #region fields
        private FileSource _mainSource;
        private List<FileSource> _mirrors;
        private int _queryIndex;
        #endregion

        #region constructors
        public SequentialMirrorSelector()
        {
        }
        #endregion

        #region properties
        public int MirrorCount
        {
            get
            {
                this.ThrowIfNotInitialized();

                return this._mirrors.Count;
            }
        }

        public FileSource MainSource
        {
            get
            {
                this.ThrowIfNotInitialized();

                return this._mainSource;
            }
        }
        #endregion

        #region public methods
        public void Initialize(Downloader downloader, FileSource mainSource, IEnumerable<FileSource> mirrors)
        {
            this._mainSource = mainSource;

            if (mirrors != null)
            {
                this._mirrors = new List<FileSource>(mirrors);
            }
            else
            {
                this._mirrors = new List<FileSource>();
            }

            this._queryIndex = 0;
        }

        public FileSource GetNextSource()
        {
            this.ThrowIfNotInitialized();

            FileSource src;

            this._queryIndex++;

            if (this._queryIndex > this._mirrors.Count) this._queryIndex = 0;

            if (this._queryIndex == 0)
            {
                src = this._mainSource;
            }
            else
            {
                src = this._mirrors[this._queryIndex - 1];
            }

            return src;
        }

        public void Remove(FileSource mirror)
        {
            this.ThrowIfNotInitialized();

            if (this._mainSource == mirror) throw new Exception("Can not remove main source.");

            this._mirrors.Remove(mirror);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.ThrowIfNotInitialized();

            var list = new List<FileSource>(this._mirrors);
            list.Insert(0, this._mainSource);

            return list.GetEnumerator();
        }

        public IEnumerator<FileSource> GetEnumerator()
        {
            this.ThrowIfNotInitialized();

            var list = new List<FileSource>(this._mirrors);
            list.Insert(0, this._mainSource);

            return list.GetEnumerator();
        }
        #endregion

        #region private methods
        private void ThrowIfNotInitialized()
        {
            if (this._mainSource == null) throw new Exception("SequentialMirrorSelector not init.");
            if (this._mirrors == null) throw new Exception("SequentialMirrorSelector not init.");
        }
        #endregion
    }
}
