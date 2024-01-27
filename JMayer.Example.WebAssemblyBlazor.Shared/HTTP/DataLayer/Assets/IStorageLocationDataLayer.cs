using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

/// <summary>
/// The interface for interacting with a remote server using CRUD operations specifically for storage locations.
/// </summary>
public interface IStorageLocationDataLayer : ISubUserEditableDataLayer<StorageLocation>
{
}
