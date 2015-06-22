using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;

namespace ConFigures.Web.Controllers
{
    public class ConfigurationController : ApiController
    {
        [Route("api/applications/{appName}/envs/{envName}")]
        public Configs GetConfig(string appName, string envName)
        {
            return JsonConvert.DeserializeObject<Configs>(ConfigAccess.GetConfig(appName, envName));
        }

        [HttpPost]
        [Route("api/applications/{appName}/envs/{envName}")]
        public void EditConfig(string appName, string envName, Configs configs)
        {
            ConfigAccess.SaveConfig(appName, envName, JsonConvert.SerializeObject(configs));
        }

        [Route("api/applications")]
        public List<string> GetApplications()
        {
            return ConfigAccess.GetAllApplications();
        }

        [HttpPost]
        [Route("api/applications")]
        public void AddApplication(AddApplicationDto app)
        {
            ConfigAccess.AddApplication(app.AppName);
        }
    }

    public class AddApplicationDto
    {
        public string AppName { get; set; }
    }

    public static class ConfigAccess
    {
        private static readonly string ConfigFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Configs");

        public static List<string> GetAllApplications()
        {
            var allAppsFile = AllAppsFileName();
            return File.Exists(allAppsFile) ? JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(allAppsFile)) : new List<string>();
        }

        public static void AddApplication(string appName)
        {
            appName = appName.Trim().ToLower();
            if (!File.Exists(AllAppsFileName()))
            {
                File.WriteAllText(AllAppsFileName(), JsonConvert.SerializeObject(new List<string> { appName }));
            }
            else
            {
                var allApps = GetAllApplications();
                if (allApps.Contains(appName))
                {
                    throw new ArgumentException("App '" + appName + "' already exists.");
                }
                allApps.Add(appName);
                File.WriteAllText(AllAppsFileName(), JsonConvert.SerializeObject(allApps));
            }
        }

        public static string GetConfig(string appName, string envName)
        {
            var filePath = BuildFilePath(appName, envName);
            if (!File.Exists(filePath)) return JsonConvert.SerializeObject(new Configs());
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

        private static string AllAppsFileName()
        {
            return ConfigFolder + "/apps.json";
        }

        private static string BuildFilePath(string appName, string envName)
        {
            return ConfigFolder + "/" + appName.Trim().ToLower() + "/" + envName.Trim().ToLower() + ".config.json";
        }
    }
}
