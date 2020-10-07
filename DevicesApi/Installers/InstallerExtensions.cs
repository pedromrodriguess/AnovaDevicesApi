using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
