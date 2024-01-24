using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using Microsoft.AspNetCore.Components;

namespace JMayer.Example.WebAssemblyBlazor.Client.Components.Base;

/// <summary>
/// The class manages interaction for an inspector.
/// </summary>
/// <typeparam name="T">Must be a UserEditableDataObject.</typeparam>
/// <typeparam name="U">Must be a IUserEditableDataLayer.</typeparam>
public class InspectorBase<T, U> : ComponentBase
    where T : UserEditableDataObject
    where U : IUserEditableDataLayer<T>
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
    /// </summary>
    [Inject]
    protected U DataLayer { get; set; }

    /// <summary>
    /// The property gets/sets the data object being inspected.
    /// </summary>
    protected T? DataObject { get; set; }

    /// <summary>
    /// The property gets/sets the index key for the data object.
    /// </summary>
    [Parameter]
    public long IndexKey { get; set; }

    /// <summary>
    /// The property gets/sets if the inspector has finished initializing.
    /// </summary>
    protected bool Initialized { get; set; }

    /// <summary>
    /// The method queries the data object based on the set index key.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override async Task OnParametersSetAsync()
    {
        DataObject = await DataLayer.GetSingleAsync(IndexKey.ToString());
        await base.OnParametersSetAsync();
        Initialized = true;
    }
}
