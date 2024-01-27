using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

/// <summary>
/// The interface for interacting with a remote server using CRUD operations specifically for part stock.
/// </summary>
public interface IStockDataLayer : ISubUserEditableDataLayer<Stock>
{
}
