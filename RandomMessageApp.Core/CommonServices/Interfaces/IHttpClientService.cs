using RandomMessageApp.Core.CommonServices.Models;

namespace RandomMessageApp.Core.CommonServices.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseResult<T>> GetAsync<T>(string url)
        where T : class;
}
