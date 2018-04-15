using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoIvp_WebApp.Common
{
    public class WConfig
    {
        private static IConfigurationRoot Instance = null;
        public static IConfigurationRoot GetInstance()
        {
            if (Instance == null)
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
#if DEBUGLOCAL
                builder.AddJsonFile("appsettings-local.json");
#else
                builder.AddJsonFile("appsettings.json");
#endif

                var config = builder.Build();
                WConfig.Instance = config;
            }
            return WConfig.Instance;

        }
    }
}
