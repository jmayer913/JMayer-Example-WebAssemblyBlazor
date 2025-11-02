using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Components.Base;

/// <summary>
/// The class manages interaction for an inspector.
/// </summary>
/// <typeparam name="T">Must be a UserEditableDataObject.</typeparam>
/// <typeparam name="U">Must be a IUserEditableDataLayer.</typeparam>
public class InspectorBase<T, U> : ComponentBase
    where T : DataObject
    where U : IStandardCRUDDataLayer<T>
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
    /// </summary>
    [Inject]
    protected U DataLayer { get; set; } = default!;

    /// <summary>
    /// The property gets/sets the data object being inspected.
    /// </summary>
    protected T? DataObject { get; set; }

    /// <summary>
    /// The property gets/sets the dialog service used for managing MudDialogs.
    /// </summary>
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

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
        try
        {
            DataObject = await DataLayer.GetSingleAsync(IndexKey);
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }

        await base.OnParametersSetAsync();
        Initialized = true;
    }
}
