using JMayer.Data.Data;
using JMayer.Data.Database.DataLayer;
using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;

/// <summary>
/// The class manages CRUD interactions with the database for a storage location.
/// </summary>
public class StorageLocationDataLayer : UserEditableMemoryDataLayer<StorageLocation>, IStorageLocationDataLayer
{
    /// <summary>
    /// The data layer for interacting with assets.
    /// </summary>
    /// <remarks>
    /// This data layer needs to register the delete event on the
    /// asset data layer. It's also used during server-side validation.
    /// </remarks>
    private readonly IAssetDataLayer _assetDataLayer;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="assetDataLayer">The data layer for interacting with assets.</param>
    public StorageLocationDataLayer(IAssetDataLayer assetDataLayer)
    {
        _assetDataLayer = assetDataLayer;
        _assetDataLayer.Deleted += AssetDataLayer_Deleted;
    }

    /// <summary>
    /// The method deletes any storage locations associated with the deleted assets.
    /// </summary>
    /// <param name="sender">The asset data layer.</param>
    /// <param name="e">The arguments which contain the deleted assets.</param>
    private async void AssetDataLayer_Deleted(object? sender, DeletedEventArgs e)
    {
        foreach (Asset asset in e.DataObjects.Cast<Asset>())
        {
            List<StorageLocation> storageLocations = await GetAllAsync(obj => obj.OwnerInteger64ID == asset.Integer64ID);
            await DeleteAsync(storageLocations);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Overriding and not calling the base because the parent class forces the Name property 
    /// to be unique but the property is not used. Also, added additional server-side validation
    /// unique to the storage location.
    /// </remarks>
    public override async Task<List<ValidationResult>> ValidateAsync(StorageLocation dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        List<ValidationResult> validationResults = dataObject.Validate();

        if (await _assetDataLayer.ExistAsync(obj => obj.Integer64ID == dataObject.OwnerInteger64ID, cancellationToken) == false)
        {
            validationResults.Add(new ValidationResult($"The {dataObject.OwnerInteger64ID} asset was not found in the data store.", [nameof(StorageLocation.OwnerInteger64ID)]));
        }

        if (await ExistAsync(obj => obj.Integer64ID != dataObject.Integer64ID && obj.LocationA == dataObject.LocationA && obj.LocationB == dataObject.LocationB && obj.LocationC == dataObject.LocationC, cancellationToken) == true)
        {
            validationResults.Add(new ValidationResult("The location already exists in the data store.", [nameof(StorageLocation.LocationA)]));
        }

        return await Task.FromResult(validationResults);
    }
}
