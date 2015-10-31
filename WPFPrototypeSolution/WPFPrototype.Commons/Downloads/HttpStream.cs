using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    internal class HttpStream : Stream
    {
        #region fields
        private Stream _stream;
        private HttpClient _client;
        private HttpRequestMessage _request;
        private HttpResponseMessage _response;
        #endregion

        #region constructors
        public HttpStream(Stream stream, HttpClient client, HttpRequestMessage request, HttpResponseMessage response)
        {
            this._stream = stream;
            this._client = client;
            this._request = request;
            this._response = response;
        }
        #endregion

        #region properties

        public override bool CanRead
        {
            get
            {
                return this._stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this._stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this._stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this._stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this._stream.Position;
            }

            set
            {
                this._stream.Position = value;
            }
        }
        #endregion

        #region public methods

        public override void Flush()
        {
            this._stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this._stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this._stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this._stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this._stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            this._stream.Dispose();
            this._response.Dispose();
            this._request.Dispose();
            this._client.Dispose();
        }
        #endregion
    }
}
