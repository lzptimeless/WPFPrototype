using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    /// <summary>
    /// 下载协议适配器
    /// </summary>
    public class HttpProtocalProvider : IProtocalProvider
    {
        #region constructors
        public HttpProtocalProvider()
        { }
        #endregion

        #region public methods
        /// <summary>
        /// 获取数据源的下载流
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="startPosition">片段起始地址</param>
        /// <param name="endPosition">片段结束地址</param>
        /// <param name="ct">用来检测是否已经取消</param>
        /// <returns></returns>
        public async Task<Stream> GetStreamAsync(FileSource source, long startPosition, long endPosition, CancellationToken ct)
        {
            // 创建下载请求
            var client = this.CreateClient(source);
            HttpRequestMessage request;
            try
            {
                request = new HttpRequestMessage(HttpMethod.Get, source.Url);
                request.Headers.Range = new RangeHeaderValue(startPosition, endPosition);
            }
            catch
            {
                client.Dispose();
                throw;
            }

            // 请求下载
            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
                ct.ThrowIfCancellationRequested(); // 检测是否已经取消
            }
            catch
            {
                request.Dispose();
                client.Dispose();
                throw;
            }

            // 获取下载流
            Stream stream;
            try
            {
                response.EnsureSuccessStatusCode();
                stream = await response.Content.ReadAsStreamAsync();
                ct.ThrowIfCancellationRequested(); // 检测是否已经取消
            }
            catch
            {
                response.Dispose();
                request.Dispose();
                client.Dispose();
                throw;
            }

            // HttpStream用包裹，方便调用者统一释放资源
            return new HttpStream(stream, client, request, response);
        }

        /// <summary>
        /// 获取下载文件的信息
        /// </summary>
        /// <param name="source"><see cref="FileSource"/></param>
        /// <param name="ct">用来检测是否已经取消</param>
        /// <returns></returns>
        public async Task<RemoteFileInfo> GetFileInfoAsync(FileSource source, CancellationToken ct)
        {
            using (var client = this.CreateClient(source))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, source.Url))
                using (HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct))
                {
                    ct.ThrowIfCancellationRequested();

                    response.EnsureSuccessStatusCode();

                    var headers = response.Headers;
                    var contentHeaders = response.Content.Headers;
                    RemoteFileInfo remoteInfo = new RemoteFileInfo();
                    remoteInfo.MimeType = contentHeaders.ContentType.MediaType; // 下载文件类型
                    remoteInfo.IsAcceptRange = headers.AcceptRanges.Contains("bytes"); // 是否支持分段下载
                    remoteInfo.Size = contentHeaders.ContentLength.Value; // 文件大小

                    if (contentHeaders.LastModified.HasValue)
                    {
                        remoteInfo.ModifyTime = contentHeaders.LastModified.Value.DateTime; // 文件修改时间
                    }

                    byte[] md5 = contentHeaders.ContentMD5;
                    if (md5 != null && md5.Length > 0)
                    {
                        remoteInfo.MD5 = Convert.ToBase64String(md5); // 文件MD5
                    }

                    return remoteInfo;
                }// using
            }// using
        }
        #endregion

        #region private methods
        /// <summary>
        /// 创建<see cref="HttpClient"/>
        /// </summary>
        /// <param name="source">下载源</param>
        /// <returns></returns>
        private HttpClient CreateClient(FileSource source)
        {
            if (source.IsNeedAuthentication)
            {
                // 如果需要域认证则添加账号密码
                string userName = source.Account;
                string domain = string.Empty;

                // 提取域和账号
                int slashIndex = userName.IndexOf('\\');
                if (slashIndex >= 0)
                {
                    domain = userName.Substring(0, slashIndex);
                    userName = userName.Substring(slashIndex + 1);
                }

                NetworkCredential cred = new NetworkCredential(userName, source.Password, domain);
                HttpClientHandler handler = new HttpClientHandler();
                handler.Credentials = cred;
                handler.PreAuthenticate = true;

                return new HttpClient(handler);
            }
            else
            {
                return new HttpClient();
            }
            #endregion
        }
    }
}
