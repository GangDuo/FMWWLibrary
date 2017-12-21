using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMWW.Core
{
    public sealed class Config
    {
        private static volatile Config instance;
        private static object syncRoot = new Object();
        private string hostName;

        public string HostName
        {
            get
            {
                if (String.IsNullOrEmpty(hostName))
                {
                    Load();
                }
                return hostName;
            }
            set
            {
                hostName = value;
            }
        }

        private Config() { }

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Config();
                    }
                }

                return instance;
            }
        }

        public void Load()
        {
            using (var sr = new StreamReader(".hostname.json"))
            {
                var text = sr.ReadToEnd();
                if (text.Length == 0)
                {
                    throw new Exception("Host未設定");
                }
                else
                {
                    HostName = Text.Json.Parse<string>(text);
                }
            }
        }
    }
}
