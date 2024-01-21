using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using Microsoft.AspNetCore.Components;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Assets;

/// <summary>
/// The class manages user interactions with the AssetInspector.razor page.
/// </summary>
public class AssetInspectorBase : ComponentBase
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
    /// </summary>
    [Inject]
    protected IAssetDataLayer DataLayer { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the data object being inspected.
    /// </summary>
    protected Asset? DataObject { get; set; }

    /// <summary>
    /// The property gets/sets the index key for the data object.
    /// </summary>
    [Parameter]
    public long IndexKey { get; set; }

    /// <summary>
    /// The method queries the data object based on the set index key.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override async Task OnParametersSetAsync()
    {
        DataObject = await DataLayer.GetSingleAsync(IndexKey.ToString());
        await base.OnParametersSetAsync();
    }
}
