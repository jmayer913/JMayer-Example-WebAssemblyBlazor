using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

/// <summary>
/// The interface for interacting with a remote server using CRUD operations specifically for parts.
/// </summary>
public interface IPartDataLayer : IUserEditableDataLayer<Part>
{
    /// <summary>
    /// The method returns all the defined categories for the parts.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of categories.</returns>
    Task<List<string>?> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
