namespace RandomMessageApp.Core.CommonServices.Models;

public sealed record HttpResponseResult<T>
    where T : class
{
    public T Result { get; set; }

    public HttpResponseError Error { get; set; }
}
