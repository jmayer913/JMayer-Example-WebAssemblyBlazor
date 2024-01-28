using JMayer.Data.Data;
using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Dialogs;

/// <summary>
/// The class manages user interactions with the AddEditStockDialog.razor dialog.
/// </summary>
public class AddEditStockDialogBase : AddEditCardDialogBase<Stock, IStockDataLayer>
{
    /// <summary>
    /// The selected storage location.
    /// </summary>
    private ListView? _selectedStorageLocation;

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
    /// The property gets/sets the storage location which the user selected.
    /// </summary>
    protected ListView? SelectedStorageLocation
    {
        get => _selectedStorageLocation;
        set
        {
            _selectedStorageLocation = value;
            //Map any selection changes to the StorageLocationId property.
            DataObject.StorageLocationId = value?.Integer64ID ?? 0;
        }
    }

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override void OnParametersSet()
    {
        //The Name property is a required field but the
        //Stock data object doesn't use it so
        //it needs to be set to pass validation.
        DataObject.Name = "A Name";
        base.OnParametersSet();
    }

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override async Task OnParametersSetAsync()
    {
        StorageLocations = await StorageLocationDataLayer.GetAllListViewAsync() ?? [];

        if (!IsNewRecord)
        {
            StorageLocation? storageLocation = await StorageLocationDataLayer.GetSingleAsync(DataObject.StorageLocationId);

            if (storageLocation != null)
            {
                SelectedStorageLocation = new ListView()
                {
                    Integer64ID = storageLocation.Integer64ID,
                    Name = storageLocation.Name,
                };
            }
        }

        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// The method returns the storage location list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<ListView>> OnStorageLocationAutoCompleteSearchAsync(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return await Task.FromResult(StorageLocations);
        }
        else
        {
            return await Task.FromResult(StorageLocations.Where(obj => obj.Name.Contains(value)));
        }
    }
}
