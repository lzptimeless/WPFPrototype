using WPFPrototype.Toolkit.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Collections.Concurrent;

namespace WPFPrototype.Toolkit.Languages
{
    /// <summary>
    /// 多语言资源管理类，所有键值忽略大小写
    /// </summary>
    public class LanguageResources : ILanguageResources
    {
        #region fields
        /// <summary>
        /// 默认多语言资源文件夹路径
        /// </summary>
        private string _resourceDirectoryPath;
        /// <summary>
        /// 缓存_sourceDefaultDirectoryPath目录下的资源名
        /// </summary>
        private List<string> _resourceNames;
        /// <summary>
        /// 当前语言资源文件路径
        /// </summary>
        private string _resourcePath;
        /// <summary>
        /// 当前语言
        /// </summary>
        private string _language;
        /// <summary>
        /// 多语言资源类型
        /// </summary>
        private LanguageResourceTypes _resourceType;
        /// <summary>
        /// 存储多语言资源
        /// </summary>
        private ConcurrentDictionary<string, object> _resources = new ConcurrentDictionary<string, object>();
        #endregion

        #region constructors
        /// <summary>
        /// 初始化<see cref="LanguageResources"/>
        /// </summary>
        /// <param name="resourceDirectoryPath">多语言资源文件夹路径</param>
        /// <param name="resourceType">多语言资源类型</param>
        public LanguageResources(string resourceDirectoryPath, LanguageResourceTypes resourceType)
        {
            this._resourceNames = new List<string>();
            this._resourceDirectoryPath = resourceDirectoryPath;
            this._resourceType = resourceType;

            // 缓存文件夹下多语言资源名称
            this.CacheResourceNames();
        }
        #endregion

        #region properties
        /// <summary>
        /// 获取当前多语言资源文件路径，可能为null
        /// </summary>
        public string ResourcePath
        {
            get { return this._resourcePath; }
        }

        /// <summary>
        /// 获取多语言资源类型
        /// </summary>
        public LanguageResourceTypes ResourceType
        {
            get { return this._resourceType; }
        }

        /// <summary>
        /// 获取多语言文字，可能为null
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                var res = this._resources;
                // key为空
                if (string.IsNullOrEmpty(key)) return null;
                // 忽略大小写
                key = key.ToLowerInvariant();
                // key不存在
                if (!res.ContainsKey(key)) return null;

                object value = res[key];
                var refKey = value as LanguageReference;
                // value为引用
                if (refKey != null && (!string.IsNullOrEmpty(refKey.Key)))
                {
                    // 引用不存在
                    if (!res.ContainsKey(refKey.Key)) return null;

                    value = res[refKey.Key];
                    // 不能连续引用两次
                    if (value is LanguageReference) throw new Exception(string.Format("Can not use ref chain, [key:{0}].", key));

                    return value as string;
                }

                // value为语言
                return value as string;
            }
        }
        #endregion

        #region events
        #region LanguageChanged
        /// <summary>
        /// Event name of <see cref="LanguageChanged"/>
        /// </summary>
        public const string LanguageChangedEventName = "LanguageChanged";

        public event EventHandler<EventArgs> LanguageChanged;

        private void OnLanguageChanged()
        {
            EventHandler<EventArgs> handler = this.LanguageChanged;

            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddWeakLanguageChangedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<LanguageResources, EventArgs>.AddHandler(this, LanguageChangedEventName, handler);
        }

        public void RemoveWeakLanguageChangedHandler(EventHandler<EventArgs> handler)
        {
            WeakEventManager<LanguageResources, EventArgs>.RemoveHandler(this, LanguageChangedEventName, handler);
        }
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// 检查默认路径下的资源是否支持这个语言，只要找到相近的语言都算作支持
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言</param>
        /// <returns></returns>
        public bool IsSupported(string ietfLanguageTag)
        {
            string sourceName = this.GetSupportedResourceName(ietfLanguageTag);
            return !string.IsNullOrEmpty(sourceName);
        }

        /// <summary>
        /// 检查当前语言是否在默认路径下存在
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言</param>
        /// <returns></returns>
        public bool IsContains(string ietfLanguageTag)
        {
            return this._resourceNames.Any(p => string.Equals(Path.GetFileNameWithoutExtension(p), ietfLanguageTag, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// 获取默认资源文件夹下所有的语言，返回ietf language tag表示的语言
        /// </summary>
        /// <returns></returns>
        public string[] GetLanguages()
        {
            return this._resourceNames.Select(p => Path.GetFileNameWithoutExtension(p)).ToArray();
        }

        /// <summary>
        /// 获取当前语言，返回ietf language tag表示的语言
        /// </summary>
        /// <returns></returns>
        public string GetLanguage()
        {
            return this._language;
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言，用来查找默认路径下多语言资源文件</param>
        public void SetLanguage(string ietfLanguageTag)
        {
            string sourcePath;
            string content = this.GetResourceContent(ietfLanguageTag, out sourcePath);
            if (string.IsNullOrEmpty(content))
                throw new FileNotFoundException(string.Format("Can not found language resources, [culture:{0}].", ietfLanguageTag));

            this.LoadResources(content);
            this._resourcePath = sourcePath;
            this._language = ietfLanguageTag;
            OnLanguageChanged();
        }
        #endregion

        #region private methods
        /// <summary>
        /// 加载本地多语言资源文件
        /// </summary>
        /// <param name="filePath">多语言资源文件内容</param>
        private void LoadResources(string content)
        {
            XDocument doc = XDocument.Parse(content);
            var elements = doc.Root.Elements();
            this._resources.Clear();
            foreach (var element in elements)
            {
                var keyAttr = element.Attribute("key");
                if (keyAttr == null || string.IsNullOrEmpty(keyAttr.Value)) continue;

                var refAttr = element.Attribute("ref");
                object value;
                if (refAttr != null && !string.IsNullOrEmpty(refAttr.Value))
                {
                    value = new LanguageReference(refAttr.Value.ToLowerInvariant());
                }
                else
                {
                    value = element.Value;
                }

                this._resources.TryAdd(keyAttr.Value.ToLowerInvariant(), value);
            }// foreach
        }

        /// <summary>
        /// 获取多语言资源内容，失败返回null
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言</param>
        /// <param name="sourcePath">资源文件路径</param>
        /// <returns></returns>
        private string GetResourceContent(string ietfLanguageTag, out string sourcePath)
        {
            sourcePath = null;
            string directory = this._resourceDirectoryPath;
            string sourceName = this.GetSupportedResourceName(ietfLanguageTag);
            if (string.IsNullOrEmpty(sourceName)) return null;

            // 加载资源内容
            string sourcePath1 = Path.Combine(directory, sourceName);
            string content = GetResourceContent(sourcePath1);
            if (!string.IsNullOrEmpty(content))
            {
                sourcePath = sourcePath1;
                return content;
            }

            return null;
        }

        /// <summary>
        /// 获取这个语言支持的资源文件名，只要找到相近的语言都算作支持
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag表示的语言</param>
        /// <returns></returns>
        private string GetSupportedResourceName(string ietfLanguageTag)
        {
            string[] tags = new[] {
                ietfLanguageTag,
                ietfLanguageTag.Split('-')[0] + '-'
            };
            var sourceNames = this._resourceNames;

            foreach (var tag in tags)
            {
                string sourceName = sourceNames.FirstOrDefault(p => p.StartsWith(tag, StringComparison.InvariantCultureIgnoreCase));
                if (!string.IsNullOrEmpty(sourceName)) return sourceName;
            }

            return null;
        }

        /// <summary>
        /// 获取所有多语言资源文件路径
        /// </summary>
        /// <returns></returns>
        private void CacheResourceNames()
        {
            LanguageResourceTypes resourceType = this._resourceType;
            string dirPath = this._resourceDirectoryPath;
            if (resourceType == LanguageResourceTypes.File)
            {
                // 如果是外部文件则直接获取所有文件路径
                this._resourceNames = Directory.GetFiles(dirPath, "*.*", SearchOption.TopDirectoryOnly)
                                    .Select(p => Path.GetFileName(p))
                                    .ToList();
                return;
            }

            // 如果是Resource和Content文件则需要从_sourceDefaultDirectoryPath中获取到Assembly和路径，并通过Assembly
            // 读取对应路径下的资源文件
            Regex pathRegex = new Regex(@"^(?<prefix>pack://application:,,,)?(/(?<assemblyName>[^;]+)(;[^;]+){0,2};component)?/(?<path>.*)$", RegexOptions.IgnoreCase);
            var mh = pathRegex.Match(dirPath);

            if (!mh.Success) throw new InvalidOperationException(string.Format("The SourceDefaultDirectoryPath is invalid: [path:{0}]", dirPath));

            Assembly asm = null;
            var assemblyNameGroup = mh.Groups["assemblyName"];
            if (!assemblyNameGroup.Success)
            {
                // 没有AssemblyName说明这个路径表示主Assembly
                asm = Assembly.GetEntryAssembly();
            }
            else
            {
                // 通过AssemblyName获取程序集
                string assemblyName = assemblyNameGroup.Value;
                asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name == assemblyName);

                if (asm == null) throw new InvalidOperationException(string.Format("Can not find resource assembly: [assembly:{0}]", assemblyName));
            }

            string path = mh.Groups["path"].Value;
            if (resourceType == LanguageResourceTypes.Content && asm == Assembly.GetEntryAssembly())
            {
                // 当Assembly为WPF工程时，并且资源类型为Content时，必须用AssemblyAssociatedContentFileAttribute
                // 才能获取资源文件列表
                this._resourceNames = asm.GetCustomAttributes(typeof(AssemblyAssociatedContentFileAttribute), true)
                                    .Cast<AssemblyAssociatedContentFileAttribute>()
                                    .Where(p => p.RelativeContentFilePath.TrimStart('/').StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                                    .Select(attr => Path.GetFileName(attr.RelativeContentFilePath))
                                    .ToList();
            }
            else
            {
                // 当Assembly不是WPF工程时获取资源文件列表需要用GetManifestResourceStream
                string resName = asm.GetName().Name + ".g.resources";
                using (var stream = asm.GetManifestResourceStream(resName))
                using (var reader = new ResourceReader(stream))
                {
                    this._resourceNames = reader.Cast<DictionaryEntry>()
                                        .Where(p => ((string)p.Key).TrimStart('/').StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                                        .Select(p => Path.GetFileName((string)p.Key))
                                        .ToList();
                }
            }// else
        }

        /// <summary>
        /// 获取多语言资源内容，失败返回null
        /// </summary>
        /// <param name="sourcePath">多语言资源文件路径</param>
        /// <returns></returns>
        private string GetResourceContent(string sourcePath)
        {
            LanguageResourceTypes resourceType = this._resourceType;

            switch (resourceType)
            {
                case LanguageResourceTypes.Resource:
                case LanguageResourceTypes.Content:
                    {
                        Uri uri = new Uri(sourcePath, UriKind.RelativeOrAbsolute);
                        StreamResourceInfo srcInfo = null;

                        if (resourceType == LanguageResourceTypes.Resource)
                            srcInfo = Application.GetResourceStream(uri);
                        else
                            srcInfo = Application.GetContentStream(uri);

                        if (srcInfo != null)
                        {
                            using (var s = srcInfo.Stream)
                            using (var r = new StreamReader(s))
                                return r.ReadToEnd();
                        }
                    }
                    break;
                case LanguageResourceTypes.File:
                    {
                        if (File.Exists(sourcePath))
                        {
                            return File.ReadAllText(sourcePath);
                        }
                    }
                    break;
                default:
                    break;
            }// switch

            return null;
        }
        #endregion
    }
}
