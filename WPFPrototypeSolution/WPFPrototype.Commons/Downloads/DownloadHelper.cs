using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WPFPrototype.Commons.Downloads
{
    public static class DownloadHelper
    {
        /// <summary>
        /// 通过下载url分析文件名，失败返回GUID.unkown文件名
        /// </summary>
        /// <param name="url">下载url</param>
        /// <returns></returns>
        public static string GetFileNameFromUrl(string url)
        {
            string defaultName = Guid.NewGuid().ToString("N") + ".unkown";

            return GetFileNameFromUrl(url, defaultName);
        }

        /// <summary>
        /// 通过下载url分析文件名
        /// </summary>
        /// <param name="url">下载url</param>
        /// <param name="defaultName">默认文件名，分析失败时返回</param>
        /// <returns></returns>
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

        /// <summary>
        /// 通过文件路径获取这个文件的下载配置文件路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetConfigPathFromFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("filePath can not be null or empty.");

            return string.Format("{0}.config", filePath);
        }

        /// <summary>
        /// 通过文件路径获取这个文件的下载临时文件路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetTmpPathFromFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("filePath can not be null or empty.");

            return string.Format("{0}.tmp", filePath);
        }

        /// <summary>
        /// 通过下载配置文件路径获取这个文件的保存路径
        /// </summary>
        /// <param name="configPath">下载配置文件路径</param>
        /// <returns></returns>
        public static string GetFilePathFromConfigPath(string configPath)
        {
            if (string.IsNullOrEmpty(configPath)) throw new ArgumentException("configPath can not be null or empty.");

            if (!configPath.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("configPath is invalid.");
            }

            return configPath.Substring(0, configPath.Length - 7);
        }
    }
}
