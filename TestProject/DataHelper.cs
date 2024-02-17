using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

namespace TestProject;

/// <summary>
/// The static class contains helper methods for data creation/retrieval.
/// </summary>
internal static class DataHelper
{
    /// <summary>
    /// The method either retrieves or creates an area asset.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="areaName">The name of the area asset.</param>
    /// <returns>An area asset or null.</returns>
    public static async Task<Asset?> GetOrCreateAreaAssetAsync(HttpClient client, string areaName)
    {
        AssetDataLayer dataLayer = new(client);

        List<Asset>? assets = await dataLayer.GetAllAsync();

        if (assets != null)
        {
            Asset? areaAsset = assets.FirstOrDefault(obj => obj.Name == areaName && obj.Type == AssetType.Area);

            if (areaAsset != null)
            {
                return areaAsset;
            }
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = areaName, Type = AssetType.Area });
        return operationResult.DataObject as Asset;
    }

    /// <summary>
    /// The method either retrieves or creates a part.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="name">The name of the part.</param>
    /// <returns>A part or null.</returns>
    public static async Task<Part?> GetOrCreatePartAsync(HttpClient client, string name)
    {
        PartDataLayer dataLayer = new(client);

        List<Part>? parts = await dataLayer.GetAllAsync();

        if (parts != null)
        {
            Part? part = parts.FirstOrDefault(obj => obj.Name == name);

            if (part != null)
            {
                return part;
            }
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = name });
        return operationResult.DataObject as Part;
    }

    /// <summary>
    /// The method either retrieves or creates a storage location.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="locationName">The name of the storage location.</param>
    /// <param name="ownerID">The area asset which owns the location.</param>
    /// <returns>A storage location or null.</returns>
    public static async Task<StorageLocation?> GetOrCreateStorageLocationAsync(HttpClient client, string locationName, long ownerID)
    {
        StorageLocationDataLayer dataLayer = new(client);

        List<StorageLocation>? storageLocations = await dataLayer.GetAllAsync();

        if (storageLocations != null)
        {
            StorageLocation? foundStorageLocation = storageLocations.FirstOrDefault(obj => obj.LocationA == locationName);

            if (foundStorageLocation != null)
            {
                return foundStorageLocation;
            }
        }

        StorageLocation storageLocation = new()
        {
            LocationA = locationName,
            LocationB = locationName,
            LocationC = locationName,
            OwnerInteger64ID = ownerID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(storageLocation);
        return operationResult.DataObject as StorageLocation;
    }
}
