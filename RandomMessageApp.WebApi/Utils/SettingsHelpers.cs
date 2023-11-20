namespace RandomMessageApp.WebApi.Utils;

public static class SettingsHelpers
{
    public static Func<IServiceProvider, T> GetSettings<T>(string section)
        where T : class
    {
        return serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();

            return configuration.GetSection(section).Get<T>();
        };
    }
}
