using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevicesApi.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerInstaller = new SwaggerInstaller();
            swaggerInstaller.InstallServices(services, configuration);

            var dbInstaller = new DbInstaller();
            dbInstaller.InstallServices(services, configuration);
        }
    }
}
