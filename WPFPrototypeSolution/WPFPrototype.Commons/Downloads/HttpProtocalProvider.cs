using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class HttpProtocalProvider : IProtocalProvider
    {
        #region constructors
        public HttpProtocalProvider()
        { }
        #endregion

        #region public methods

        public async Task<Stream> CreateStreamAsync(FileSource source, long startPosition, long endPosition)
        {
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

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            }
            catch
            {
                request.Dispose();
                client.Dispose();
                throw;
            }

            Stream stream;
            try
            {
                response.EnsureSuccessStatusCode();
                stream = await response.Content.ReadAsStreamAsync();
            }
            catch
            {
                response.Dispose();
                request.Dispose();
                client.Dispose();
                throw;
            }

            return new HttpStream(stream, client, request, response);
        }

        public async Task<RemoteFileInfo> GetFileInfoAsync(FileSource source)
        {
            using (var client = this.CreateClient(source))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, source.Url))
                using (HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var headers = response.Headers;
                    var contentHeaders = response.Content.Headers;
                    RemoteFileInfo remoteInfo = new RemoteFileInfo();
                    remoteInfo.MimeType = contentHeaders.ContentType.MediaType;
                    remoteInfo.IsAcceptRange = headers.AcceptRanges.Contains("bytes");
                    remoteInfo.Size = contentHeaders.ContentLength.Value;

                    if (contentHeaders.LastModified.HasValue)
                    {
                        remoteInfo.ModifyTime = contentHeaders.LastModified.Value.DateTime;
                    }

                    if (contentHeaders.ContentMD5 != null && contentHeaders.ContentMD5.Length > 0)
                    {
                        remoteInfo.MD5 = Encoding.UTF8.GetString(contentHeaders.ContentMD5);
                    }
                    return remoteInfo;
                }// using
            }// using
        }

        public void Init(Downloader downloader)
        {
            // 没有什么需要做的
        }

        #endregion

        #region private methods
        private HttpClient CreateClient(FileSource source)
        {
            if (source.IsNeedAuthentication)
            {
                string userName = source.Account;
                string domain = string.Empty;

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
