namespace RandomMessageApp.Core.CommonServices.Interfaces;

public interface IJsonSerializationService
{
    string Serialize(object obj);

    T Deserialize<T>(string content);

    T Deserialize<T>(Stream stream);
}
