using System.IO;
using System.Web.Http;
using ConFigures.Web.Models;
using Newtonsoft.Json;

namespace ConFigures.Web.Controllers
{
    public class ConfigurationController : ApiController
    {
        [Route("api/applications/{appName}/envs/{envName}")]
        public Configs Get(string appName, string envName)
        {
            return JsonConvert.DeserializeObject<Configs>(ConfigAccess.GetConfig(appName, envName));
        }

        [Route("api/applications/{appName}/envs/{envName}")]
        public void Post(string appName, string envName, Configs configs)
        {
            ConfigAccess.SaveConfig(appName, envName, JsonConvert.SerializeObject(configs));
        }
    }

    public static class ConfigAccess
    {
        private static readonly string ConfigFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Configs");

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

        private static string BuildFilePath(string appName, string envName)
        {
            return ConfigFolder + "/" + appName + "/" + envName + ".config";
        }
    }
}
