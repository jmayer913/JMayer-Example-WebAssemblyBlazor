using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

/// <summary>
/// The class manages CRUD interactions with a remote server for a storage location.
/// </summary>
public class StorageLocationDataLayer : UserEditableDataLayer<StorageLocation>, IStorageLocationDataLayer
{
    /// <inheritdoc/>
    public StorageLocationDataLayer(HttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// The method returns a page of remote data objects.
    /// </summary>
    /// <param name="assetId">The asset to filter for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    public async Task<PagedList<StorageLocation>?> GetPageAsync(long assetId, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<StorageLocation>? pagedList = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"/api/{TypeName}/Page/{assetId}?{queryDefinition.ToQueryString()}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            pagedList = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<StorageLocation>>(cancellationToken);
        }

        return pagedList;
    }

    /// <summary>
    /// The method returns a page of remote data objects.
    /// </summary>
    /// <param name="assetId">The asset to filter for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    public async Task<PagedList<ListView>?> GetPageListViewAsync(long assetId, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<ListView>? pagedList = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"/api/{TypeName}/Page/ListView/{assetId}?{queryDefinition.ToQueryString()}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            pagedList = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<ListView>>(cancellationToken);
        }

        return pagedList;
    }
}
