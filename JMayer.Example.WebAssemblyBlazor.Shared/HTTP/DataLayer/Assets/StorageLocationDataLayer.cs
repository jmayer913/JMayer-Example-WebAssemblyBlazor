using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

/// <summary>
/// The class manages CRUD interactions with a remote server for a storage location.
/// </summary>
public class StorageLocationDataLayer : SubUserEditableDataLayer<StorageLocation>, IStorageLocationDataLayer
{
    /// <inheritdoc/>
    public StorageLocationDataLayer(HttpClient httpClient) : base(httpClient) { }
}
