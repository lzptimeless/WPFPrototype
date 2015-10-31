using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public interface IProtocalProvider
    {
        void Init(Downloader downloader);

        Task<Stream> CreateStreamAsync(FileSource source, long startPosition, long endPosition);

        Task<RemoteFileInfo> GetFileInfoAsync(FileSource source);
    }
}
