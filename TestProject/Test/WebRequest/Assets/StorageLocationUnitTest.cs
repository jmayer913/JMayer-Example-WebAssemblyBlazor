using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TestProject.Test.WebRequest.Assets;

/// <summary>
/// The class manages tests for parts using both the http client and server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class StorageLocationUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public StorageLocationUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method confirms the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CountStorageLocationsAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        int count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a storage location to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="areaName">The name of the area asset.</param>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Area 1", "Test Location A", "", "")]
    [InlineData("Test Area 2", "Test Location A", "Test Location B", "")]
    [InlineData("Test Area 3", "Test Location A", "Test Location B", "Test Location C")]
    public async Task AddStorageLocationAsync(string areaName, string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, areaName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation originalDataObject = new()
        {
            LocationA = locationA,
            LocationB = locationB,
            LocationC = locationC,
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        originalDataObject.Name = originalDataObject.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        Assert.True
        (
            operationResult.IsSuccessStatusCode //The operation must have been successful.
            && operationResult.DataObject is StorageLocation returnedDataObject //A storage location must have been returned.
            && new StorageLocationEqualityComparer(true).Equals(returnedDataObject, originalDataObject) //Original and return must be equal.
        );
    }

    /// <summary>
    /// The method confirms on the server-side if an area asset is deleted, the associated storage locations are also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteAreaAssetAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Cascade Storage Location Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Cascade Storage Location Delete Test 1", Name = "Cascade Storage Location Delete Test 1", OwnerInteger64ID = areaAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the storage location.");
        }

        operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Cascade Storage Location Delete Test 2", Name = "Cascade Storage Location Delete Test 2", OwnerInteger64ID = areaAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the storage location.");
        }

        operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Cascade Storage Location Delete Test 3", Name = "Cascade Storage Location Delete Test 3", OwnerInteger64ID = areaAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the storage location.");
        }

        await new AssetDataLayer(client).DeleteAsync(areaAsset);

        List<StorageLocation>? storageLocations = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        Assert.True(storageLocations != null && storageLocations.Count == 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a storage location to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteStorageLocationAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Storage Location Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Delete Test", Name = "Test", OwnerInteger64ID = areaAsset.Integer64ID });

        if (operationResult.DataObject is StorageLocation dataObject)
        {
            operationResult = await dataLayer.DeleteAsync(dataObject);
            Assert.True(operationResult.IsSuccessStatusCode);
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
        }
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all storage locations as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewStorageLocationsAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        List<ListView>? listViews = await dataLayer.GetAllListViewAsync();
        Assert.True(listViews != null && listViews.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all storage locations for an area asset as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewStorageLocationsWithOwnerIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, BHSExampleBuilder.MainPartStorageAreaAssetName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to find the area asset");
        }

        List<ListView>? dataObjects = await dataLayer.GetAllListViewAsync(areaAsset.Integer64ID);
        Assert.True(dataObjects != null && dataObjects.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all storage locations from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllStorageLocationsAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        List<StorageLocation>? dataObjects = await dataLayer.GetAllAsync();
        Assert.True(dataObjects != null && dataObjects.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all storage locations for an area asset from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllStorageLocationsWithOwnerIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, BHSExampleBuilder.MainPartStorageAreaAssetName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to find the area asset");
        }

        List<StorageLocation>? dataObjects = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        Assert.True(dataObjects != null && dataObjects.Count > 0);
    }

    /// <summary>
    /// The method either retrieves or creates an area asset.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="areaName">The name of the area asset.</param>
    /// <returns>An area asset or null.</returns>
    private static async Task<Asset?> GetOrCreateAreaAssetAsync(HttpClient client, string areaName)
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
    /// The method confirms the HTTP data layer can request the first storage location from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleStorageLocationAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        StorageLocation? dataObject = await dataLayer.GetSingleAsync();
        Assert.NotNull(dataObject);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a storage location from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleStorageLocationWithIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Storage Location Single Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Single Test", Name = "Test", OwnerInteger64ID = areaAsset.Integer64ID });

        if (operationResult.DataObject is StorageLocation storageLocation)
        {
            StorageLocation? dataObject = await dataLayer.GetSingleAsync(storageLocation.Integer64ID);
            Assert.NotNull(dataObject);
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
        }
    }
}
