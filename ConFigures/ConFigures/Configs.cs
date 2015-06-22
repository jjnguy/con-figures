using System.Collections.Generic;

namespace ConFigures
{
    public class Configs
    {
        public Dictionary<string, string> AppSettings { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }

        public Configs()
        {
            AppSettings = new Dictionary<string, string>();
            ConnectionStrings = new Dictionary<string, string>();
        }
    }
}
