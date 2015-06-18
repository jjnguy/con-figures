using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConFigures.Client
{
    public class ConfigurationClient
    {
        private IKeyValuePairCollection _appSettings;
        private IKeyValuePairCollection _connectionStrings;

        public ConfigurationClient()
        {
        }

        public IKeyValuePairCollection AppSettings
        {
            get { return _appSettings; }
        }
    }

    public interface IKeyValuePairCollection
    {
        string Get(string key);
    }
}
