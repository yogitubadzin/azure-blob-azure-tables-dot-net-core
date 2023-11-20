using AutoMapper;
using RandomMessageApp.Application.Services.Implementation;
using RandomMessageApp.Application.Services.Interfaces;
using RandomMessageApp.Core.CommonServices.Implementation;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.Storage.Configuration;
using RandomMessageApp.Storage.Services.Implementation;
using RandomMessageApp.Storage.Services.Interfaces;
using RandomMessageApp.WebApi.Mapping;
using RandomMessageApp.WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = new MapperConfiguration(c => {
    c.AddProfile<RandomMessagesProfile>();
});
builder.Services.AddSingleton<IMapper>(s => config.CreateMapper());

builder.Services.AddSingleton(SettingsHelpers.GetSettings<StorageSettings>("Storage"));
builder.Services.AddScoped<IJsonSerializationService, JsonSerializationService>();
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddScoped<ITableStorage, TableStorage>();
builder.Services.AddScoped<IDateTimeService, DateTimeService>();
builder.Services.AddScoped<IRandomMessagesService, RandomMessagesService>();
builder.Services.AddScoped<IPartitionKeyGenerator, PartitionKeyGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();