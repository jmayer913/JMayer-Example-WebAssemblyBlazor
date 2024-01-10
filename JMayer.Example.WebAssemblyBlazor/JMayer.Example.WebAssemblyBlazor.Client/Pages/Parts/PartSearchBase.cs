using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts;

/// <summary>
/// The class manages user interactions with the PartSearch.razor page.
/// </summary>
public class PartSearchBase : ComponentBase
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
    /// </summary>
    [Inject]
    protected IPartDataLayer DataLayer { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the dialog service used for managing MudDialogs.
    /// </summary>
    [Inject]
    protected IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// The property gets/sets a reference to the UI data grid.
    /// </summary>
    protected MudDataGrid<Part> MudDataGrid { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the navigation manager which is used to navigate to the inspector.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// The method queries a page of parts based on user interactions with the grid.
    /// </summary>
    /// <param name="gridState">Tells the server how to query data.</param>
    /// <returns>A page of parts.</returns>
    protected async Task<GridData<Part>> OnDataGridStateChangedAsync(GridState<Part> gridState)
    {
        try
        {
            List<Part>? parts = await DataLayer.GetPageAsync(gridState.ToQueryDefinition());

            if (parts != null)
            {
                return new GridData<Part>()
                {
                    TotalItems = parts.Count,
                    Items = parts,
                };
            }
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }

        return new GridData<Part>();
    }

    /// <summary>
    /// The method attempts to delete a part if the user confirms the action.
    /// </summary>
    /// <param name="part">The part to delete.</param>
    /// <returns>A Task object for the async.</returns>
    protected async Task OnDeleteButtonClickAsync(Part part)
    {
        bool? result = await DialogService.ShowConfirmActionMessageAsync();

        if (result == true)
        {
            try
            {
                OperationResult operationResult = await DataLayer.DeleteAsync(part);

                if (operationResult.IsSuccessStatusCode)
                {
                    await MudDataGrid.ReloadServerData();
                }
                else
                {
                    await DialogService.ShowErrorMessageAsync("Failed to delete the part because of an error on the server.");
                }
            }
            catch
            {
                await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
            }
        }
    }

    /// <summary>
    /// The method navigates to the inspector page.
    /// </summary>
    /// <param name="part">The part to inspect.</param>
    protected void OnEditButtonClick(Part part)
    {
        NavigationManager.NavigateTo($"/Part/{part.Integer64ID}");
    }

    /// <summary>
    /// The method opens a dialog for creating a new part and if not canceled, the list is required from the server.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected async Task OnNewButtonClickAsync()
    {
        IDialogReference dialogReference = await DialogService.ShowAsync<NewPartDialog>("Create a New Part");
        DialogResult dialogResult = await dialogReference.Result;

        if (!dialogResult.Canceled)
        {
            await MudDataGrid.ReloadServerData();
        }
    }
}
