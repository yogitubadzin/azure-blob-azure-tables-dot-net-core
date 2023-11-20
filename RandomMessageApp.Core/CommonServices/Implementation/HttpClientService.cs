using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.Core.CommonServices.Models;

namespace RandomMessageApp.Core.CommonServices.Implementation;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IJsonSerializationService _jsonSerializationService;

    public HttpClientService(
        IHttpClientFactory clientFactory,
        IJsonSerializationService jsonSerializationService)
    {
        _clientFactory = clientFactory;
        _jsonSerializationService = jsonSerializationService;
    }

    public async Task<HttpResponseResult<T>> GetAsync<T>(string url)
        where T : class
    {
        var client = _clientFactory.CreateClient();
        try
        {
            var response = await client.GetAsync(new Uri(url));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = _jsonSerializationService.Deserialize<T>(responseContent);

            if (result == null)
            {
                throw new ArgumentNullException("Response result is null.");
            }

            return new HttpResponseResult<T>
            {
                Result = result
            };
        }
        catch (Exception exception)
        {
            return new HttpResponseResult<T>
            {
                Error = new HttpResponseError
                {
                    Message = exception.Message
                }
            };
        }
    }
}
