using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SteamConsoleUploader
{
    public class SteamConfig
    {
        Dictionary<string, string> _RawDict = new Dictionary<string, string>();
        public string SDKPath
        {
            get
            {
                var path = GetValue("umake.steam.sdkpath");
                return NormalizedPath(path);
            }
        }
        private string NormalizedPath(string path)
        {
            return path.Replace(@"\:", ":").Replace(@"\","/");
        }
        public string Login => GetValue("umake.steam.login");
        public string Password => GetValue("umake.android.password");
        public string BuildScript
        {
            get
            {
                var path =GetValue("umake.steam.buildscript");
                return NormalizedPath(path);
            }
        }
        public string BuildContentRelativeToBuildScript => GetValue("umake.steam.subfoldercontent");
        public string BuildPath
        {
            get
            {
                var exePath = GetValue("umake.buildpath");

                return Directory.GetParent(exePath).FullName;
            }
        }

        public SteamConfig(Dictionary<string, string> raw)
        {
            _RawDict = raw;
        }

        public static SteamConfig FromFilePath(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new Exception($"Cannot find file at path {path}");
                }

                var dict = new Dictionary<string, string>();
                var lines = File.ReadAllLines(path);
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    sb.AppendLine(line);
                    var split = line.Split('=');
                    if (split.Length >= 2)
                    {
                        dict.Add(split[0], split[1]);
                    }
                }

                var s = new SteamConfig(dict);
                return s;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetValue(string key, string defaultValue = "")
        {
            if (_RawDict == null)
                return "";
            if (_RawDict.TryGetValue(key, out var ret))
                return ret;
            return defaultValue;
        }
    }
}