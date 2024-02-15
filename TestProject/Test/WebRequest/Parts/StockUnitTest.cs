using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TestProject.Test.WebRequest.Parts;

/// <summary>
/// The class manages tests for stock for parts using both the http client and server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class StockUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public StockUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method confirms the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CountPartsAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        int count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a stock to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="name">The name of the part.</param>
    /// <param name="description">The description for the part.</param>
    /// <param name="category">The common category for the part.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Stock Area 1", "Test Location 1", "Stock Part 1", 10)]
    [InlineData("Test Stock Area 2", "Test Location 2", "Stock Part 2", 0.25)]
    [InlineData("Test Stock Area 3", "Test Location 3", "Stock Part 3", 1234.673)]
    public async Task AddStockAsync(string areaName, string locationName, string partName, decimal amount)
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, areaName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation? storageLocation = await GetOrCreateStorageLocationAsync(client, locationName, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
        }

        Part? part = await GetOrCreatePartAsync(client, partName);

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
        }

        Stock originalDataObject = new()
        {
            Amount = amount,
            Name = "Create Test",
            OwnerInteger64ID = part.Integer64ID,
            StorageLocationId = storageLocation.Integer64ID,
            StorageLocationName = storageLocation.FriendlyName,
        };
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        Assert.True
        (
            operationResult.IsSuccessStatusCode //The operation must have been successful.
            && operationResult.DataObject is Stock returnedDataObject //A stock must have been returned.
            && new StockEqualityComparer(true).Equals(returnedDataObject, originalDataObject)
        );
    }

    /// <summary>
    /// The method confirms on the server-side if an area asset is deleted, the associated storage location is also deleted which also deletes the associated stock.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteAreaAssetAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Cascade Area Asset-Stock Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation? storageLocation = await GetOrCreateStorageLocationAsync(client, "Cascade Area Asset-Stock Delete Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
        }

        Part? part = await GetOrCreatePartAsync(client, "Cascade Area Asset-Stock Delete Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Cascade Area Asset-Stock Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationId = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the stock.");
        }

        await new AssetDataLayer(client).DeleteAsync(areaAsset);

        List<Stock>? stocks = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        Assert.True(stocks != null && stocks.Count == 0);
    }

    /// <summary>
    /// The method confirms on the server-side if a part is deleted, the associated stock is also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeletePartAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Cascade Part-Stock Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation? storageLocation = await GetOrCreateStorageLocationAsync(client, "Cascade Part-Stock Delete Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
        }

        Part? part = await GetOrCreatePartAsync(client, "Cascade Part-Stock Delete Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Cascade Part-Stock Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationId = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the stock.");
        }

        await new PartDataLayer(client).DeleteAsync(part);

        List<Stock>? stocks = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        Assert.True(stocks != null && stocks.Count == 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request stock to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteStockAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Stock Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation? storageLocation = await GetOrCreateStorageLocationAsync(client, "Stock Delete Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
        }

        Part? part = await GetOrCreatePartAsync(client, "Stock Delete Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationId = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (operationResult.DataObject is Stock stock)
        {
            operationResult = await dataLayer.DeleteAsync(stock);
            Assert.True(operationResult.IsSuccessStatusCode);
        }
        else
        {
            Assert.Fail("Failed to crete the stock.");
        }
    }

    /// <summary>
    /// The method confirms on the server-side if a storage location is deleted, the associated stock is also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteStorageLocationAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Cascade Storage Location-Stock Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation? storageLocation = await GetOrCreateStorageLocationAsync(client, "Cascade Storage Location-Stock Delete Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
        }

        Part? part = await GetOrCreatePartAsync(client, "Cascade Storage Location-Stock Delete Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Cascade Storage Location-Stock Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationId = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the stock.");
        }

        await new StorageLocationDataLayer(client).DeleteAsync(storageLocation);

        List<Stock>? stocks = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        Assert.True(stocks != null && stocks.Count == 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all stocks as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewStockAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<ListView>? listViews = await dataLayer.GetAllListViewAsync();
        Assert.True(listViews != null && listViews.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all stocks for a specifc part as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewStockWithIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<ListView>? listViews = await dataLayer.GetAllListViewAsync(2);
        Assert.True(listViews != null && listViews.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all stocks from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllStockAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<Stock>? dataObjects = await dataLayer.GetAllAsync();
        Assert.True(dataObjects != null && dataObjects.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all stocks for a specific part from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllStockWithIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<Stock>? dataObjects = await dataLayer.GetAllAsync(1);
        Assert.True(dataObjects != null && dataObjects.Count > 0 && dataObjects.All(obj => obj.OwnerInteger64ID == 1));
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
    /// The method either retrieves or creates a part.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="name">The name of the part.</param>
    /// <returns>A part or null.</returns>
    private static async Task<Part?> GetOrCreatePartAsync(HttpClient client, string name)
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
    private static async Task<StorageLocation?> GetOrCreateStorageLocationAsync(HttpClient client, string locationName, long ownerID)
    {
        StorageLocationDataLayer dataLayer = new(client);

        List<StorageLocation>? storageLocations = await dataLayer.GetAllAsync();

        if (storageLocations != null)
        {
            StorageLocation? foundStorageLocation = storageLocations.FirstOrDefault(obj => obj.Name == locationName);

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

    /// <summary>
    /// The method confirms the HTTP data layer can request the first stock from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleStockAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Stock? dataObject = await dataLayer.GetSingleAsync();
        Assert.NotNull(dataObject);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request stock from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleStockWithIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await GetOrCreateAreaAssetAsync(client, "Stock Single Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
        }

        StorageLocation? storageLocation = await GetOrCreateStorageLocationAsync(client, "Stock Single Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
        }

        Part? part = await GetOrCreatePartAsync(client, "Stock Single Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Single Test", OwnerInteger64ID = part.Integer64ID, StorageLocationId = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (operationResult.DataObject is Stock stock)
        {
            Stock? dataObject = await dataLayer.GetSingleAsync(stock.Integer64ID);
            Assert.NotNull(dataObject);
        }
        else
        {
            Assert.Fail("Failed to crete the stock.");
        }
    }
}
