using JMayer.Data.Database.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;

/// <summary>
/// The interface for interacting with a storage location collection in a database using CRUD operations.
/// </summary>
public interface IStorageLocationDataLayer : IUserEditableDataLayer<StorageLocation>
{
}
