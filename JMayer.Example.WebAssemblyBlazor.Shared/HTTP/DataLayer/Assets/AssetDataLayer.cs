using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using System.Net.Http.Json;
using System.Net;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

/// <summary>
/// The class manages CRUD interactions with a remote server for an asset.
/// </summary>
public class AssetDataLayer : UserEditableDataLayer<Asset>, IAssetDataLayer
{
    /// <inheritdoc/>
    public AssetDataLayer(HttpClient httpClient) : base(httpClient) { }

    /// <inheritdoc/>
    public async Task<List<string>?> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        List<string>? categories = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Category/All", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            categories = await httpResponseMessage.Content.ReadFromJsonAsync<List<string>>(cancellationToken);
        }

        return categories;
    }
}
