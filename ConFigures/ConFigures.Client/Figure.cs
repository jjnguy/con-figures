﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConFigures
{
    public static class Figure
    {
        public static void Start(string envName)
        {
            var appSettings = new XElement("appSettings");
            AddNode(appSettings, "add", Tuple.Create("key", "Example"), Tuple.Create("value", envName));
            var config = new XElement("configuration");
            config.Add(appSettings);
            var doc = new XDocument();
            doc.Add(config);

            var fileName = Path.GetTempFileName();

            doc.Save(fileName);

            AppConfig.Change(fileName);
        }

        private static void AddNode(XElement toAddTo, string nodeName, params Tuple<string, string>[] attrs)
        {
            var newNode = new XElement(nodeName);
            foreach (var tuple in attrs)
            {
                newNode.SetAttributeValue(tuple.Item1, tuple.Item2);
            }
            toAddTo.Add(newNode);
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