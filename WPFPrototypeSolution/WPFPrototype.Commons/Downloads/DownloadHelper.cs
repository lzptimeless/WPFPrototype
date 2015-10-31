using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WPFPrototype.Commons.Downloads
{
    public static class DownloadHelper
    {
        public static string GetFileNameFromUrl(string url)
        {
            string defaultName = Guid.NewGuid().ToString("N") + ".unkown";

            return GetFileNameFromUrl(url, defaultName);
        }

        public static string GetFileNameFromUrl(string url, string defaultName)
        {
            if (string.IsNullOrEmpty(url)) return defaultName;
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return defaultName;

            Uri uri = new Uri(url);

            if (uri.Segments == null || uri.Segments.Length == 0) return defaultName;

            string fileName = HttpUtility.UrlDecode(uri.Segments.Last()).Replace("/", "_").Trim();

            if (string.IsNullOrEmpty(fileName)) return defaultName;

            return fileName;
        }
    }
}
