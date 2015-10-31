using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public interface IMirrorSelector:IEnumerable<FileSource>
    {
        FileSource Source { get; }

        int MirrorCount { get; }

        void Init(Downloader downloader, FileSource source, IEnumerable<FileSource> mirrors);

        FileSource GetNextSource();

        void Remove(FileSource mirror);
    }
}
