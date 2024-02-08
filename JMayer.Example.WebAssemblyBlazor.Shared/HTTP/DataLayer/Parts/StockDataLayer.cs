using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

/// <summary>
/// The class manages CRUD interactions with a remote server for a part stock.
/// </summary>
public class StockDataLayer : SubUserEditableDataLayer<Stock>, IStockDataLayer
{
    /// <inheritdoc/>
    public StockDataLayer(HttpClient httpClient) : base(httpClient) { }
}
