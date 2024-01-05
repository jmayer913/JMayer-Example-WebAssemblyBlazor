using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

/// <summary>
/// The class manages CRUD interactions with a remote server for a part.
/// </summary>
public class PartDataLayer : UserEditableDataLayer<Part>, IPartDataLayer
{
    /// <inheritdoc/>
    public PartDataLayer(HttpClient httpClient) : base(httpClient) { }

    /// <inheritdoc/>
    public async Task<List<string>?> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        List<string>? categories = null;
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"api/{_typeName}/Category/All", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            categories = await httpResponseMessage.Content.ReadFromJsonAsync<List<string>>(cancellationToken);
        }

        return categories;
    }
}
