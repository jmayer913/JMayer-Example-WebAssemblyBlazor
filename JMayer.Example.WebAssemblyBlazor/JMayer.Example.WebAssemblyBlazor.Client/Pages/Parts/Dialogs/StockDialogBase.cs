using JMayer.Data.Data;
using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Dialogs;

/// <summary>
/// The class manages user interactions with the StockDialog.razor dialog.
/// </summary>
public class StockDialogBase : CardDialogBase<Stock, IStockDataLayer>
{
    /// <summary>
    /// The property gets/sets the data layer needed to access storage locations.
    /// </summary>
    [Inject]
    protected IStorageLocationDataLayer StorageLocationDataLayer { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the storage locations to choose as a stock location.
    /// </summary>
    protected List<ListView> StorageLocations { get; set; } = [];

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            StorageLocations = await StorageLocationDataLayer.GetAllListViewAsync() ?? [];
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }

        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// The method returns the storage location list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <param name="cancellationToken">Used to cancel the task.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<long>> OnStorageLocationAutoCompleteSearchAsync(string value, CancellationToken cancellationToken)
    {
        await Task.Delay(1);

        if (string.IsNullOrWhiteSpace(value))
        {
            return await Task.FromResult(StorageLocations.Select(obj => obj.Integer64ID));
        }
        else
        {
            return await Task.FromResult(StorageLocations.Where(obj => obj.Name.Contains(value)).Select(obj => obj.Integer64ID));
        }
    }

    /// <inheritdoc/>
    /// <remarks>Overridden so the stock's StorageLocationName is updated before it's submitted to the server.</remarks>
    protected override async Task OnSubmitEditFormAsync()
    {
        ListView? selected = StorageLocations.FirstOrDefault(obj => obj.Integer64ID == DataObject.StorageLocationID);
        DataObject.StorageLocationName = selected?.Name ?? string.Empty;
        await base.OnSubmitEditFormAsync();
    }
}
