using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.CustomersCanvas.Infrastructure;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;

namespace Nop.Plugin.Widgets.CustomersCanvas.Data
{
    public class EditorData
    {
        public static bool IsValidEditorFolder(string path)
        {
            return Directory.Exists(path) && File.Exists(Path.Combine(path, PluginPaths.EditorConfigFileName));
        }

        public string Uid { get; private set; }
        public string Title { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }

        public string FolderName { get; private set; }
        public string VirtualPath { get; private set; }

        private string FullPath { get; set; }
        private string FullPathToJson { get; set; }

        public ConfigData DefaultConfig { get; private set; }
        public List<ConfigData> AllConfigs { get; private set; }

        public ConfigData GetConfigByTitle(string title)
        {
            return AllConfigs.FirstOrDefault(x => x.Title == title);
        }

        public string ToJson()
        {
            return File.ReadAllText(FullPathToJson);
        }

        public EditorData()
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(Name + "_" + Version));
                Guid result = new Guid(hash);
                Uid = result.ToString().Replace("-", "");
            }
        }

        public EditorData(string path)
        {
            FullPath = path;
            FullPathToJson = Path.Combine(path, PluginPaths.EditorConfigFileName);
            using (StreamReader r = new StreamReader(FullPathToJson))
            {
                string json = r.ReadToEnd();
                var metainfo = JToken.Parse(json);
                Name = (metainfo["name"] ?? "").ToString();
                Title = (metainfo["title"] ?? "").ToString();
                Version = (metainfo["version"] ?? "").ToString();
                FolderName = Path.GetFileName(path.TrimEnd('/', '\\')); // Utils.ReverseMapPath(FullPath, PluginPaths);
                AllConfigs = new List<ConfigData>();
                VirtualPath = "~/" + path.Replace(HostingEnvironment.ApplicationPhysicalPath, String.Empty);

                var configFiles = Directory.GetFiles(Path.Combine(path, PluginPaths.ConfigsFolder));
                foreach (var configFile in configFiles)
                {
                    AllConfigs.Add(new ConfigData(configFile));
                }

                DefaultConfig = AllConfigs.FirstOrDefault(x => x.FileName == metainfo["defaultConfig"].ToString());

            }
        }
    }

    public class ConfigData
    {
        public string Uid;
        public string Title;
        public string FileName;

        public string Name;
        public string Version;

        public string FullPath;
        public string Json;

        public string ToJson()
        {
            return File.ReadAllText(FullPath);
        }

        public ConfigData()
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(Name + "_" + Version));
                Guid result = new Guid(hash);
                Uid = result.ToString().Replace("-", "");
            }
        }

        public ConfigData(string path) : this()
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var metainfo = JToken.Parse(json);

                Name = (metainfo["name"] ?? "").ToString();
                Title = (metainfo["title"] ?? "").ToString();
                Version = (metainfo["version"] ?? "").ToString();
                FileName = Path.GetFileName(path);
                FullPath = path;
                Json = json;
            }
        }
    }
}
