using CMFDev.Toolkit.Configuration.AppData;
using CMFDev.Toolkit.Configuration.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMFDev.Toolkit.Configuration.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDataConfiguration(this IServiceCollection services, Action<AppDataConfig>? configAction= null)  
        {
            return _RegisterConfiguration(services, configAction, out _);
        }

        public static IServiceCollection AddAppDataConfiguration<TAppSettingsImpl>(this IServiceCollection services, Action<AppDataConfig>? configAction) where TAppSettingsImpl : class
        {
            return _RegisterConfiguration(services, configAction, out var iconfigInstance)
                .AddSingleton<IAppDataSettingsService<TAppSettingsImpl>, AppDataSettingsService<TAppSettingsImpl>>()
                .Configure<TAppSettingsImpl>(iconfigInstance);
        }

        private static IServiceCollection _RegisterConfiguration(IServiceCollection services, Action<AppDataConfig>? configAction, out IConfiguration configInstance)
        {
            var appDataConfig = new AppDataConfig()
            {
                AppDataSource = AppDataSource.Local,
                FileName = "appsettings.json",
                Optional = true,
                PathInAppData = null,
                ReloadOnChange = true,
            };
            configAction?.Invoke(appDataConfig);

            var appDataDirectory = string.Empty;
            switch (appDataConfig.AppDataSource)
            {
                case AppDataSource.Local: appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); break;
                case AppDataSource.Roaming: appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); break;
            }

            var pathInAppData = (string.IsNullOrWhiteSpace(appDataConfig.PathInAppData) ? System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name : appDataConfig.PathInAppData) ?? string.Empty;

            var path = Path.Combine(appDataDirectory, pathInAppData);


            var configBuilder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(Path.Combine(path, appDataConfig.FileName), optional: appDataConfig.Optional, reloadOnChange: appDataConfig.ReloadOnChange);
            foreach (var additionalFile in appDataConfig.AdditionalFileNames)
            {
                configBuilder.AddJsonFile(Path.IsPathRooted(additionalFile) ? additionalFile : Path.Combine(path, additionalFile), optional: true, reloadOnChange: appDataConfig.ReloadOnChange);
            }

            configInstance = configBuilder.Build();

            return services
                .AddSingleton(appDataConfig)
                .AddSingleton(configInstance);
        }
    }
}
