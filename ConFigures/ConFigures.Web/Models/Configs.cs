using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConFigures.Web.Models
{
    public class Configs
    {
        public Dictionary<string, string> AppSettings { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}
