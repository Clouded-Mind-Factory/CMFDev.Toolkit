using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFDev.Toolkit.Configuration.AppData
{
    public class AppDataConfig
    {
        public AppDataConfig()
        {
            AppDataSource = AppDataSource.Local;
            FileName = "appsettings.json";
            Optional = true;
            PathInAppData = null;
            ReloadOnChange = true;
            AdditionalFileNames = new List<string>();
        }

        public AppDataSource AppDataSource { get; set; }
        public bool EnableVolatileStorage { get; set; }
        public string FileName { get; set; }
        public bool Optional { get; set; }
        public string? PathInAppData { get; set; }

        public bool ReloadOnChange { get; set; }

        public ICollection<string> AdditionalFileNames { get; }

    }
}
