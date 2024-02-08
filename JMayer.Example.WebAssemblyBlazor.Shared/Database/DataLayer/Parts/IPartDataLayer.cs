using JMayer.Data.Database.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;

/// <summary>
/// The interface for interacting with a parts collection in a database using CRUD operations.
/// </summary>
public interface IPartDataLayer : IUserEditableDataLayer<Part>
{
}
