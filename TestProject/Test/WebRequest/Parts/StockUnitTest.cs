using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

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
    /// The method verifies the server will return a failure if the stock (part & storage location) already exists when adding a new stock.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyAddDuplicateStockFailure()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, Constants.TestStorageLocation, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, "Duplicate Stock Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Duplicate Stock Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first stock.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Duplicate Stock Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        //The operation must have failed.
        Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

        //No asset was returned.
        Assert.Null(operationResult.DataObject);

        //A bad request status was returned.
        Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

        //A validation error was returned.
        Assert.NotNull(operationResult.ServerSideValidationResult);
        Assert.Single(operationResult.ServerSideValidationResult.Errors);

        //The correct error was returned.
        Assert.Contains("stock location already exists", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
        Assert.Equal(nameof(Stock.StorageLocationID), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a stock to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="locationName">The name of the storage location.</param>
    /// <param name="partName">The name of the part for the storage location.</param>
    /// <param name="amount">The amount of parts for the storage location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Location 1", "Stock Part 1", 10)]
    [InlineData("Test Location 2", "Stock Part 2", 0.25)]
    [InlineData("Test Location 3", "Stock Part 3", 1234.673)]
    public async Task VerifyAddStock(string locationName, string partName, decimal amount)
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, locationName, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, partName);

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        Stock partStock = new()
        {
            Amount = amount,
            Name = "Create Test",
            OwnerInteger64ID = part.Integer64ID,
            StorageLocationID = storageLocation.Integer64ID,
            StorageLocationName = storageLocation.FriendlyName,
        };
        OperationResult operationResult = await dataLayer.CreateAsync(partStock);

        Assert.True(operationResult.IsSuccessStatusCode , "The operation should have been successful."); //The operation must have been successful.
        Assert.IsType<Stock>(operationResult.DataObject); //A stock must have been returned.
        Assert.True(new StockEqualityComparer(true, true, true).Equals((Stock)operationResult.DataObject, partStock), "The data object sent should be the same as the data object returned."); //The original data matches the returned data.
    }

    /// <summary>
    /// The method verifies the server will return a failure if the part or storage location doesn't exists when adding a new stock.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyAddStockDependenciesNotExists()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);
        
        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Add Dependencies Not Exists Stock Test", OwnerInteger64ID = 0, StorageLocationID = 0 });

        //The operation must have failed.
        Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

        //No asset was returned.
        Assert.Null(operationResult.DataObject);

        //A bad request status was returned.
        Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

        //A validation error was returned.
        Assert.NotNull(operationResult.ServerSideValidationResult);
        Assert.Equal(2, operationResult.ServerSideValidationResult.Errors.Count);

        //The correct error was returned.
        Assert.Contains("part was not found", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
        Assert.Equal(nameof(Stock.OwnerInteger64ID), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
        Assert.Contains("storage location was not found", operationResult.ServerSideValidationResult.Errors[1].ErrorMessage);
        Assert.Equal(nameof(Stock.StorageLocationID), operationResult.ServerSideValidationResult.Errors[1].PropertyName);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyCountParts()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        long count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method verifies on the server-side if an area asset is deleted, the associated storage location is also deleted which also deletes the associated stock.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeleteAreaAssetCascade()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, "Cascade Asset-Stock Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, "Cascade Asset-Stock Delete Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, Constants.TestPart);

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Cascade Asset-Stock Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the stock.");
            return;
        }

        await new AssetDataLayer(client).DeleteAsync(areaAsset);

        List<Stock>? stocks = await dataLayer.GetAllAsync(areaAsset.Integer64ID);

        //The stock under the area asset was deleted.
        Assert.NotNull(stocks);
        Assert.Empty(stocks);
    }

    /// <summary>
    /// The method verifies on the server-side if a part is deleted, the associated stock is also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeletePartCascade()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, Constants.TestStorageLocation, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, "Cascade Part-Stock Delete Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Cascade Part-Stock Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the stock.");
            return;
        }

        await new PartDataLayer(client).DeleteAsync(part);

        List<Stock>? stocks = await dataLayer.GetAllAsync(areaAsset.Integer64ID);

        //The stock for the part was deleted.
        Assert.NotNull(stocks);
        Assert.Empty(stocks);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request stock to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeleteStock()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, Constants.TestStorageLocation, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, "Delete Stock Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Delete Stock Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

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
    /// The method verifies on the server-side if a storage location is deleted, the associated stock is also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeleteStorageLocationCascade()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, "Cascade Storage Location-Stock Delete Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, Constants.TestPart);

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Cascade Storage Location-Stock Delete Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the stock.");
            return;
        }

        await new StorageLocationDataLayer(client).DeleteAsync(storageLocation);

        List<Stock>? stocks = await dataLayer.GetAllAsync(areaAsset.Integer64ID);

        //The stock under the storage location was deleted.
        Assert.NotNull(stocks);
        Assert.Empty(stocks);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all stocks as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewStock()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<ListView>? partStocks = await dataLayer.GetAllListViewAsync();

        //Stock list views must have been returned.
        Assert.NotNull(partStocks);
        Assert.NotEmpty(partStocks);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all stocks for a specifc part as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewStockWithId()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<ListView>? partStocks = await dataLayer.GetAllListViewAsync(2);

        //Stock list views must have been returned.
        Assert.NotNull(partStocks);
        Assert.NotEmpty(partStocks);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all stocks from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllStock()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<Stock>? partStocks = await dataLayer.GetAllAsync();

        //Stock must have been returned.
        Assert.NotNull(partStocks);
        Assert.NotEmpty(partStocks);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all stocks for a specific part from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllStockWithId()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        List<Stock>? partStocks = await dataLayer.GetAllAsync(1);

        //Stock must have been returned.
        Assert.NotNull(partStocks);
        Assert.NotEmpty(partStocks);
        Assert.All(partStocks, stock => Assert.Equal(1, stock.OwnerInteger64ID));
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request the first stock from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSingleStock()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Stock? partStock = await dataLayer.GetSingleAsync();
        Assert.NotNull(partStock);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request stock from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSingleStockWithId()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, Constants.TestStorageLocation, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, "Get Single Stock Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock() { Amount = 0, Name = "Get Single Stock Test", OwnerInteger64ID = part.Integer64ID, StorageLocationID = storageLocation.Integer64ID, StorageLocationName = storageLocation.FriendlyName });

        if (operationResult.DataObject is Stock createdPartStock)
        {
            Stock? fetchedPartStock = await dataLayer.GetSingleAsync(createdPartStock.Integer64ID);
            Assert.NotNull(fetchedPartStock);
        }
        else
        {
            Assert.Fail("Failed to crete the stock.");
        }
    }

    /// <summary>
    /// The method verifies on the server-side if a storage location is renamed, the associated stock is updated with the new name.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyRenameStorageLocationCascade()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, "Rename Storage Location Test", areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, Constants.TestPart);

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock()
        {
            Amount = 0,
            Name = "Rename Storage Location Test",
            OwnerInteger64ID = part.Integer64ID,
            StorageLocationID = storageLocation.Integer64ID,
            StorageLocationName = storageLocation.FriendlyName,
        });

        if (operationResult.DataObject is Stock createdPartStock)
        {
            storageLocation.LocationA = "New Storage Location Test";
            operationResult = await new StorageLocationDataLayer(client).UpdateAsync(storageLocation);

            if (!operationResult.IsSuccessStatusCode)
            {
                Assert.Fail("Failed to update the storage location.");
                return;
            }

            Stock? fetchedPartStock = await dataLayer.GetSingleAsync(createdPartStock.Integer64ID);

            //The stock's storage location name must have been renamed too.
            Assert.NotNull(fetchedPartStock);
            Assert.Equal(storageLocation.FriendlyName, fetchedPartStock.StorageLocationName);
        }
        else
        {
            Assert.Fail("Failed to create the stock.");
            return;
        }
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a stock to be updated by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="locationName">The name of the storage location.</param>
    /// <param name="partName">The name of the part for the storage location.</param>
    /// <param name="amount">The amount of parts for the storage location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Location 10", "Stock Part 10", 10)]
    [InlineData("Test Location 20", "Stock Part 20", 0.25)]
    [InlineData("Test Location 30", "Stock Part 30", 1234.673)]
    public async Task VerifyUpdateStock(string locationName, string partName, decimal amount)
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, locationName, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, partName);

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock()
        {
            Amount = 0,
            Name = "Test",
            OwnerInteger64ID = part.Integer64ID,
            StorageLocationID = storageLocation.Integer64ID,
            StorageLocationName = storageLocation.FriendlyName,
        });

        if (operationResult.IsSuccessStatusCode && operationResult.DataObject is Stock createdPartStock)
        {
            Stock updatedPartStock = new(createdPartStock)
            {
                Amount = amount
            };
            operationResult = await dataLayer.UpdateAsync(updatedPartStock);

            Assert.True(operationResult.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
            Assert.IsType<Stock>(operationResult.DataObject); //A stock must have been returned.
            Assert.True(new StockEqualityComparer(true, true, true).Equals((Stock)operationResult.DataObject, updatedPartStock), "The data object sent should be the same as the data object returned."); //The original data matches the returned data.
        }
        else
        {
            Assert.Fail("Failed to create the stock.");
        }
    }

    /// <summary>
    /// The method verifies the server will return a failure if the part or storage location doesn't exists when updating a stock.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyUpdateStockDependenciesNotExists()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, Constants.TestStorageLocation, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, "Update Dependencies Not Exists Stock Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock()
        {
            Amount = 0,
            Name = "Update Dependencies Not Exists Stock Test",
            OwnerInteger64ID = part.Integer64ID,
            StorageLocationID = storageLocation.Integer64ID,
            StorageLocationName = storageLocation.FriendlyName,
        });

        if (operationResult.DataObject is Stock stock)
        {
            stock.OwnerInteger64ID = 0;
            stock.StorageLocationID = 0;
            operationResult = await dataLayer.UpdateAsync(stock);

            //The operation must have failed.
            Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

            //No asset was returned.
            Assert.Null(operationResult.DataObject);

            //A bad request status was returned.
            Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

            //A validation error was returned.
            Assert.NotNull(operationResult.ServerSideValidationResult);
            Assert.Equal(2, operationResult.ServerSideValidationResult.Errors.Count);

            //The correct error was returned.
            Assert.Contains("part was not found", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
            Assert.Equal(nameof(Stock.OwnerInteger64ID), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
            Assert.Contains("storage location was not found", operationResult.ServerSideValidationResult.Errors[1].ErrorMessage);
            Assert.Equal(nameof(Stock.StorageLocationID), operationResult.ServerSideValidationResult.Errors[1].PropertyName);
        }
        else
        {
            Assert.Fail("Failed to create the stock.");
            return;
        }
    }

    /// <summary>
    /// The method verifies the server will return a failure if the stock being updated is old.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateStockOldDataAsync()
    {
        HttpClient client = _factory.CreateClient();
        StockDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = await DataHelper.GetOrCreateStorageLocationAsync(client, Constants.TestStorageLocation, areaAsset.Integer64ID);

        if (storageLocation == null)
        {
            Assert.Fail("Failed to retrieve or create the storage location.");
            return;
        }

        Part? part = await DataHelper.GetOrCreatePartAsync(client, "Stock Old Data Test");

        if (part == null)
        {
            Assert.Fail("Failed to retrieve or create the part.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new Stock()
        {
            Amount = 0,
            Name = "Stock Old Data Test",
            OwnerInteger64ID = part.Integer64ID,
            StorageLocationID = storageLocation.Integer64ID,
            StorageLocationName = storageLocation.FriendlyName,
        });

        if (operationResult.DataObject is Stock firstPartStock)
        {
            Stock secondPartStock = new(firstPartStock);

            firstPartStock.Amount = 5;
            secondPartStock.Amount = 10;

            operationResult = await dataLayer.UpdateAsync(secondPartStock);

            if (!operationResult.IsSuccessStatusCode)
            {
                Assert.Fail("Failed to update the second stock.");
                return;
            }

            operationResult = await dataLayer.UpdateAsync(firstPartStock);

            Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed."); //The operation must have failed.
            Assert.Null(operationResult.DataObject); //No asset was returned.
            Assert.Equal(HttpStatusCode.Conflict, operationResult.StatusCode); //A conflict status was returned.
        }
        else
        {
            Assert.Fail("Failed to create the stock.");
            return;
        }
    }
}
