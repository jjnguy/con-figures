using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConFigures
{
    public static class Figure
    {
        public static void Start(string serverUrl, string appName, string envName)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = client.GetAsync(serverUrl + "/api/applications/" + appName + "/envs/" + envName).Result;
            var configFile = JsonConvert.DeserializeObject<Configs>(result.Content.ReadAsStringAsync().Result);

            var xDoc = ConfigsToConfigFile(configFile);

            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, xDoc.ToString());

            AppConfig.Change(fileName);
        }

        private static XDocument ConfigsToConfigFile(Configs conf)
        {
            var appSettings = new XElement("appSettings");
            foreach (var appSetting in conf.AppSettings)
            {
                // If the actual configuration file already has a key, we will skip it
                if (ConfigurationManager.AppSettings.AllKeys.Contains(appSetting.Key)) continue;
                var add = new XElement("add");
                add.SetAttributeValue("key", appSetting.Key);
                add.SetAttributeValue("value", appSetting.Value);
                appSettings.Add(add);
            }

            // inject original app settings
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                var add = new XElement("add");
                add.SetAttributeValue("key", key);
                add.SetAttributeValue("value", ConfigurationManager.AppSettings[key]);
                appSettings.Add(add);
            }

            var configuration = new XElement("configuration");
            configuration.Add(appSettings);
            var doc = new XDocument();
            doc.Add(configuration);
            return doc;
        }
    }

    // the following code was ripped verbatim from: http://stackoverflow.com/a/6151688/2598
    public abstract class AppConfig : IDisposable
    {
        public static AppConfig Change(string path)
        {
            return new ChangeAppConfig(path);
        }

        public abstract void Dispose();

        private class ChangeAppConfig : AppConfig
        {
            private readonly string oldConfig =
                AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

            private bool disposedValue;

            public ChangeAppConfig(string path)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
                ResetConfigMechanism();
            }

            public override void Dispose()
            {
                if (!disposedValue)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", oldConfig);
                    ResetConfigMechanism();


                    disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }

            private static void ResetConfigMechanism()
            {
                typeof(ConfigurationManager)
                    .GetField("s_initState", BindingFlags.NonPublic |
                                             BindingFlags.Static)
                    .SetValue(null, 0);

                typeof(ConfigurationManager)
                    .GetField("s_configSystem", BindingFlags.NonPublic |
                                                BindingFlags.Static)
                    .SetValue(null, null);

                typeof(ConfigurationManager)
                    .Assembly.GetTypes()
                    .Where(x => x.FullName ==
                                "System.Configuration.ClientConfigPaths")
                    .First()
                    .GetField("s_current", BindingFlags.NonPublic |
                                           BindingFlags.Static)
                    .SetValue(null, null);
            }
        }
    }
}
