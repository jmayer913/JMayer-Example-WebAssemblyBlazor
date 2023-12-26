using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Part;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Part;

/// <summary>
/// The class manages CRUD interactions with a remote server for a part.
/// </summary>
public class PartDataLayer : UserEditableDataLayer<PartDataObject>
{
    /// <inheritdoc/>
    public PartDataLayer(HttpClient httpClient) : base(httpClient) { }
}
