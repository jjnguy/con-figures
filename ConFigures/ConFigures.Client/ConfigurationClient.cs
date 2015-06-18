using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConFigures.Client
{
    public class ConfigurationClient
    {
        private AppSettings _appSettings;

        public ConfigurationClient()
        {
            _appSettings = new AppSettings();
        }

        public AppSettings AppSettings
        {
            get { return _appSettings; }
        }
    }

    public class AppSettings
    {
        public string Get(string key)
        {

        }
    }

    public class ConnectionStrings
    {
        
    }
}
