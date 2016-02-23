using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class SourceProvider
    {
        #region fields
        /// <summary>
        /// 文件源选择器
        /// </summary>
        private IMirrorSelector _mirrorSelector;
        /// <summary>
        /// 下载协议适配器
        /// </summary>
        private IProtocalProvider _protocalProvider;
        /// <summary>
        /// 要下载的文件信息
        /// </summary>
        private RemoteFileInfo _remoteFileInfo;
        #endregion

        #region constructors
        public SourceProvider(IMirrorSelector mirrorSelector, IProtocalProvider protocalProvider)
        {
            if (mirrorSelector == null) throw new ArgumentNullException("mirrorSelector");
            if (protocalProvider == null) throw new ArgumentNullException("protocalProvider");

            this._mirrorSelector = mirrorSelector;
            this._protocalProvider = protocalProvider;
        }
        #endregion

        #region properties

        #endregion

        #region public methods
        /// <summary>
        /// 初始化，从主服务器获取文件信息
        /// </summary>
        /// <param name="ct">检测是否已经取消</param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken ct)
        {
            var mainSource = this._mirrorSelector.MainSource;
            if (mainSource == null) throw new Exception("Main source is null.");

            this._remoteFileInfo = await this._protocalProvider.GetFileInfoAsync(mainSource, ct);
            ct.ThrowIfCancellationRequested(); // 检测是否已经取消
        }

        /// <summary>
        /// 获取要下载的文件信息
        /// </summary>
        /// <returns></returns>
        public RemoteFileInfo GetRemoteFileInfo()
        {
            this.ThrowIfNotInitialized();

            return this._remoteFileInfo;
        }

        /// <summary>
        /// 获取下载文件流，自动选择镜像
        /// </summary>
        /// <param name="startPosition">数据起始位置</param>
        /// <param name="endPosition">数据结束位置</param>
        /// <param name="ct">用于检测是否已经取消</param>
        /// <returns></returns>
        public async Task<Stream> GetStreamAsync(long startPosition, long endPosition, CancellationToken ct)
        {
            while (true)
            {
                var source = this._mirrorSelector.GetNextSource();
                if (source == null) throw new Exception("Can not get source.");

                RemoteFileInfo remoteFileInfo;
                try
                {
                    remoteFileInfo = await this._protocalProvider.GetFileInfoAsync(source, ct);
                    ct.ThrowIfCancellationRequested(); // 检测是否已经取消
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    this._mirrorSelector.Remove(source); // 这个数据源不可用，移除掉
                    continue;
                }
                
                if (!RemoteFileInfo.IsSameFile(remoteFileInfo, this._remoteFileInfo))
                {
                    // 这个数据源的文件信息与主数据源不同，移除这个数据源
                    this._mirrorSelector.Remove(source);
                    continue;
                }

                Stream downloadStream = null;
                try
                {
                    downloadStream = await this._protocalProvider.GetStreamAsync(source, startPosition, endPosition, ct);
                    ct.ThrowIfCancellationRequested(); // 检测是否已经取消
                }
                catch (OperationCanceledException)
                {
                    if (downloadStream != null) downloadStream.Dispose(); // 释放资源
                    throw;
                }
                catch (Exception)
                {
                    if (downloadStream != null) downloadStream.Dispose(); // 释放资源
                    this._mirrorSelector.Remove(source); // 这个数据源不可用，移除掉
                    continue;
                }

                return downloadStream;
            }// while
        }
        #endregion

        #region private methods
        private void ThrowIfNotInitialized()
        {
            if (this._remoteFileInfo == null) throw new Exception("Not initialized.");
        }
        #endregion
    }
}
