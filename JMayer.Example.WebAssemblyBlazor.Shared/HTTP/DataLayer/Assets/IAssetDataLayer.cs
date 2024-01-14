using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

/// <summary>
/// The interface for interacting with a remote server using CRUD operations specifically for assets.
/// </summary>
public interface IAssetDataLayer : IUserEditableDataLayer<Asset>
{
    /// <summary>
    /// The method returns all the defined categories for the assets.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of categories.</returns>
    Task<List<string>?> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
