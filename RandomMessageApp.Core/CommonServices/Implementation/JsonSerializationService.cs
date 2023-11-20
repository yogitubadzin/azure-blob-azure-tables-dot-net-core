using System.Text.Json;
using RandomMessageApp.Core.CommonServices.Interfaces;

namespace RandomMessageApp.Core.CommonServices.Implementation;

public class JsonSerializationService : IJsonSerializationService
{
    public string Serialize(object obj)
    {
        var options = CreateJsonSerializerOptions();

        return JsonSerializer.Serialize(obj, options);
    }

    public T Deserialize<T>(string content)
    {
        var options = CreateJsonSerializerOptions();

        return JsonSerializer.Deserialize<T>(content, options);
    }

    public T Deserialize<T>(Stream stream)
    {
        var options = CreateJsonSerializerOptions();

        return JsonSerializer.Deserialize<T>(stream, options);
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
