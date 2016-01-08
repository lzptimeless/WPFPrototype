using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class LocalFileWriter
    {
        #region fields
        private LocalFileInfo _info;
        private FileStream _stream;
        #endregion

        #region constructors
        public LocalFileWriter(LocalFileInfo info)
        {
            this._info = info;
        }
        #endregion

        #region properties

        #endregion

        #region public methods
        /// <summary>
        /// 创建本地文件，如果已经存在则不创建，设置文件大小
        /// </summary>
        public void CreateFile()
        {
            var info = this._info;
            string path = info.SavePath;
            long size = info.RemoteInfo.Size;

            this._stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            if (this._stream.Length != size)
            {
                this._stream.SetLength(size);
            }
        }

        /// <summary>
        /// 写入数据到本地
        /// </summary>
        /// <param name="buffer">数据buffer</param>
        /// <param name="positon">写入位置</param>
        /// <param name="bufferOffset">数据在buffer的起始位置</param>
        /// <param name="length">写入大小</param>
        public void Write(byte[] buffer, long positon, int bufferOffset, int length)
        {
            if (this._stream == null) throw new Exception("LocalFileWriter not open local stream.");

            lock(this._stream)
            {
                if (this._stream.Position != positon)
                {
                    // 设置写入位置
                    this._stream.Seek(positon, SeekOrigin.Begin);
                }
                // 写入数据到本地
                this._stream.Write(buffer, bufferOffset, length);
            }// lock
        }
        #endregion

        #region private methods

        #endregion
    }
}
