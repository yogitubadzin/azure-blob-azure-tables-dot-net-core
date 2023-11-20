using System;
using System.IO;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RandomMessageApp.Core.CommonServices.Implementation;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.FunctionApp;
using RandomMessageApp.FunctionApp.Configuration;
using RandomMessageApp.FunctionApp.Services.Implementation;
using RandomMessageApp.FunctionApp.Services.Interfaces;
using RandomMessageApp.Storage.Configuration;
using RandomMessageApp.Storage.Services.Implementation;
using RandomMessageApp.Storage.Services.Interfaces;

[assembly: FunctionsStartup(typeof(Startup))]

namespace RandomMessageApp.FunctionApp;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var configPath = Path.Combine(executableLocation, "..");

        builder.ConfigurationBuilder
            .AddJsonFile("local.settings.json", true)
            .AddJsonFile(Path.Combine(configPath, "appsettings.json"), true)
            .Build();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton(GetSettings<RandomMessageServiceSettings>("RandomMessageService"));
        builder.Services.AddSingleton(GetSettings<StorageSettings>("Storage"));

        builder.Services.AddScoped<IRandomMessageService, RandomMessageService>();
        builder.Services.AddScoped<IHttpClientService, HttpClientService>();
        builder.Services.AddScoped<IJsonSerializationService, JsonSerializationService>();
        builder.Services.AddScoped<IFileStorage, FileStorage>();
        builder.Services.AddScoped<ITableStorage, TableStorage>();
        builder.Services.AddScoped<IDateTimeService, DateTimeService>();
        builder.Services.AddScoped<ITableStoragePrimaryKeyGenerator, TableStoragePrimaryKeyGenerator>();
        builder.Services.AddScoped<IPartitionKeyGenerator, PartitionKeyGenerator>();
    }

    private static Func<IServiceProvider, T> GetSettings<T>(string section)
        where T: class
    {
        return serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();

            return configuration.GetSection(section).Get<T>();
        };
    }
}
