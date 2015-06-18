using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Web.Http;
using System.Web.Http.Results;
using System.Xml.Linq;

namespace ConFigures.Controllers
{
    public class ConfigurationController : ApiController
    {
        [Route("api/applications/{appName}/envs/{envName}")]
        public HttpResponseMessage Get(string appName, string envName)
        {
            var content = new StringContent(ConfigAccess.GetConfig(appName, envName));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };
        }

        [Route("api/applications/{appName}/envs/{envName}")]
        public void Post(string appName, string envName)
        {
            ConfigAccess.SaveConfig(appName, envName, Request.Content.ReadAsStringAsync().Result);
        }
    }

    public static class ConfigAccess
    {
        private static readonly string ConfigFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Configs");


        public static string GetConfig(string appName, string envName)
        {
            var filePath = BuildFilePath(appName, envName);
            if (!File.Exists(filePath)) return "<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration></configuration>";
            return File.ReadAllText(filePath);
        }

        public static void SaveConfig(string appName, string envName, string content)
        {
            var filePath = BuildFilePath(appName, envName);
            if (!Directory.Exists(Directory.GetParent(filePath).FullName))
            {
                Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
            }
            File.WriteAllText(filePath, content);
        }

        private static string BuildFilePath(string appName, string envName)
        {
            return ConfigFolder + "/" + appName + "/" + envName + ".config";
        }
    }
}
