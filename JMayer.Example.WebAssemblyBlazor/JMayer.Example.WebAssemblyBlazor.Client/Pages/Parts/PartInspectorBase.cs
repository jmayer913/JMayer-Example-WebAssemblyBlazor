using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts;

/// <summary>
/// The class manages user interactions with the PartInspector.razor page.
/// </summary>
public class PartInspectorBase : ComponentBase
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
    /// </summary>
    [Inject]
    protected IPartDataLayer DataLayer { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the index key for the part.
    /// </summary>
    [Parameter]
    public long IndexKey { get; set; }

    /// <summary>
    /// The property gets/sets the part being inspected.
    /// </summary>
    protected Part? Part { get; set; }

    /// <summary>
    /// The method queries the part based on the set index key.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override async Task OnParametersSetAsync()
    {
        Part = await DataLayer.GetSingleAsync(IndexKey.ToString());
        await base.OnParametersSetAsync();
    }
}
