using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class SequentialMirrorSelector : IMirrorSelector
    {
        #region fields
        private FileSource _source;
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
                this.CheckInit();

                return this._mirrors.Count;
            }
        }

        public FileSource Source
        {
            get
            {
                this.CheckInit();

                return this._source;
            }
        }
        #endregion

        #region public methods
        public void Init(Downloader downloader, FileSource source, IEnumerable<FileSource> mirrors)
        {
            this._source = source;

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
            this.CheckInit();

            FileSource src;

            this._queryIndex++;

            if (this._queryIndex > this._mirrors.Count) this._queryIndex = 0;

            if (this._queryIndex == 0)
            {
                src = this._source;
            }
            else
            {
                src = this._mirrors[this._queryIndex - 1];
            }

            return src;
        }

        public void Remove(FileSource mirror)
        {
            this.CheckInit();

            if (this._source == mirror) return;

            this._mirrors.Remove(mirror);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.CheckInit();

            var list = new List<FileSource>(this._mirrors);
            list.Insert(0, this._source);

            return list.GetEnumerator();
        }

        public IEnumerator<FileSource> GetEnumerator()
        {
            this.CheckInit();

            var list = new List<FileSource>(this._mirrors);
            list.Insert(0, this._source);

            return list.GetEnumerator();
        }
        #endregion

        #region private methods
        private void CheckInit()
        {
            if (this._source == null) throw new Exception("SequentialMirrorSelector not init.");
            if (this._mirrors == null) throw new Exception("SequentialMirrorSelector not init.");
        }
        #endregion
    }
}
