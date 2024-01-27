using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Components.Base;

/// <summary>
/// The class manages user interaction for an editable card.
/// </summary>
/// <typeparam name="T">Must be a UserEditableDataObject.</typeparam>
/// <typeparam name="U">Must be a IUserEditableDataLayer.</typeparam>
/// <typeparam name="V">Must be a component that's also a dialog.</typeparam>
public class AddEditCardBase<T, U, V, X> : ComponentBase
    where T : SubUserEditableDataObject
    where U : UserEditableDataObject
    where V : ISubUserEditableDataLayer<T>
    where X : ComponentBase
{
    /// <summary>
    /// The property gets/sets the data layer to used by the card.
    /// </summary>
    [Inject]
    protected V DataLayer { get; set; }

    /// <summary>
    /// The property gets/sets the data object the card will display information for.
    /// </summary>
    [Parameter]
    public U DataObject { get; set; } = null!;

    /// <summary>
    /// The property gets/sets a change event for the data object.
    /// </summary>
    [Parameter]
    public EventCallback<U> DataObjectChanged { get; set; }

    /// <summary>
    /// The name of the data object.
    /// </summary>
    private readonly string DataObjectTypeName = typeof(T).Name;

    /// <summary>
    /// The property gets/sets the dialog service used for managing MudDialogs.
    /// </summary>
    [Inject]
    protected IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// The property gets/sets a reference to the UI data grid.
    /// </summary>
    protected MudDataGrid<T> MudDataGrid { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the navigation manager which is used to navigate to the inspector.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// The method queries a page of data objects based on user interactions with the grid.
    /// </summary>
    /// <param name="gridState">Tells the server how to query data.</param>
    /// <returns>A page of data objects.</returns>
    protected virtual async Task<GridData<T>> OnDataGridStateChangedAsync(GridState<T> gridState)
    {
        try
        {
            PagedList<T>? pagedDataObjects = await DataLayer.GetPageAsync(DataObject.Integer64ID.ToString(), gridState.ToQueryDefinition());

            if (pagedDataObjects != null)
            {
                return new GridData<T>()
                {
                    TotalItems = pagedDataObjects.TotalRecords,
                    Items = pagedDataObjects.DataObjects,
                };
            }
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }

        return new GridData<T>();
    }

    /// <summary>
    /// The method attempts to delete a data object if the user confirms the action.
    /// </summary>
    /// <param name="dataObject">The data object to delete.</param>
    /// <returns>A Task object for the async.</returns>
    protected virtual async Task OnDeleteButtonClickAsync(T dataObject)
    {
        bool? result = await DialogService.ShowConfirmActionMessageAsync();

        if (result == true)
        {
            try
            {
                OperationResult operationResult = await DataLayer.DeleteAsync(dataObject);

                if (operationResult.IsSuccessStatusCode)
                {
                    await MudDataGrid.ReloadServerData();
                }
                else
                {
                    await DialogService.ShowErrorMessageAsync("Failed to delete the object because of an error on the server.");
                }
            }
            catch
            {
                await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
            }
        }
    }

    /// <summary>
    /// The method opens a dialog for editing a data object and if not canceled, refreshes the data grid.
    /// </summary>
    /// <param name="dataObject">The data object to inspect.</param>
    protected virtual async Task OnEditButtonClickAsync(T dataObject)
    {
        DialogParameters dialogParameters = new()
        {
            { "DataObject", dataObject },
            { "IsNewRecord", false },
        };
        IDialogReference dialogReference = await DialogService.ShowAsync<X>($"Edit the {DataObjectTypeName}", dialogParameters);
        DialogResult dialogResult = await dialogReference.Result;

        if (!dialogResult.Canceled)
        {
            await MudDataGrid.ReloadServerData();
        }
    }

    /// <summary>
    /// The method opens a dialog for creating a new data object and if not canceled, refreshes the data grid.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected virtual async Task OnNewButtonClickAsync()
    {
        DialogParameters dialogParameters = new()
        {
            { "IsNewRecord", true },
            { "OwnerId", DataObject.Integer64ID },
        };
        IDialogReference dialogReference = await DialogService.ShowAsync<X>($"Create a New {DataObjectTypeName}", dialogParameters);
        DialogResult dialogResult = await dialogReference.Result;

        if (!dialogResult.Canceled)
        {
            await MudDataGrid.ReloadServerData();
        }
    }
}
