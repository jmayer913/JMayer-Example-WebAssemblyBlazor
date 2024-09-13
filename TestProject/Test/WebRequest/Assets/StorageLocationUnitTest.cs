using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

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
    /// The method verifies the server will return a failure if the storage location already exists when adding a new asset.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Duplicate Storage Location Test A", "", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "Duplicate Storage Location Test C")]
    public async Task VerifyAddDuplicateStorageLocationFailure(string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation storageLocation = new()
        {
            LocationA = locationA,
            LocationB = locationB,
            LocationC = locationC,
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(storageLocation);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first storage location.");
            return;
        }

        storageLocation = new()
        {
            LocationA = locationA,
            LocationB = locationB,
            LocationC = locationC,
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        operationResult = await dataLayer.CreateAsync(storageLocation);

        //The operation must have failed.
        Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

        //No storage location was returned.
        Assert.Null(operationResult.DataObject);

        //A bad request status was returned.
        Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

        //A validation error was returned.
        Assert.NotNull(operationResult.ServerSideValidationResult);
        Assert.Single(operationResult.ServerSideValidationResult.Errors);

        //The correct error was returned.
        Assert.Contains("location already exists", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
        Assert.Equal(nameof(StorageLocation.LocationA), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a storage location to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Location A", "", "")]
    [InlineData("Test Location A", "Test Location B", "")]
    [InlineData("Test Location A", "Test Location B", "Test Location C")]
    public async Task VerifyAddStorageLocation(string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation storageLocation = new()
        {
            LocationA = locationA,
            LocationB = locationB,
            LocationC = locationC,
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(storageLocation);

        Assert.True(operationResult.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.IsType<StorageLocation>(operationResult.DataObject); //A storage location must have been returned.
        Assert.True(new StorageLocationEqualityComparer(true, true, true).Equals((StorageLocation)operationResult.DataObject, storageLocation), "The data object sent should be the same as the data object returned."); //Original and return must be equal.
    }

    /// <summary>
    /// The method verifies the server will return a failure if the area asset doesn't exists when adding a new storage location.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyAddStorageLocationDependenciesNotExists()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Add Dependencies Not Exists Storage Location Test", Name = "Dependencies Not Exists Storage Location Test", OwnerInteger64ID = 0 });

        //The operation must have failed.
        Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

        //No storage location was returned.
        Assert.Null(operationResult.DataObject);

        //A bad request status was returned.
        Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

        //A validation error was returned.
        Assert.NotNull(operationResult.ServerSideValidationResult);
        Assert.Single(operationResult.ServerSideValidationResult.Errors);

        //The correct error was returned.
        Assert.Contains("asset was not found", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
        Assert.Equal(nameof(StorageLocation.OwnerInteger64ID), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyCountStorageLocations()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        long count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method verifies on the server-side if an area asset is deleted, the associated storage locations are also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeleteAreaAssetCascade()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, "Cascade Area Asset-Storage Location Delete Test");

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Cascade Area Asset-Storage Location Delete Test 1", Name = "Cascade Storage Location Delete Test 1", OwnerInteger64ID = areaAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the storage location.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Cascade Area Asset-Storage Location Delete Test 2", Name = "Cascade Storage Location Delete Test 2", OwnerInteger64ID = areaAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the storage location.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Cascade Area Asset-Storage Location Delete Test 3", Name = "Cascade Storage Location Delete Test 3", OwnerInteger64ID = areaAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the storage location.");
            return;
        }

        await new AssetDataLayer(client).DeleteAsync(areaAsset);

        List<StorageLocation>? storageLocations = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        
        Assert.NotNull(storageLocations);
        Assert.Empty(storageLocations);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a storage location to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeleteStorageLocation()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Delete Storage Location Test", Name = "Test", OwnerInteger64ID = areaAsset.Integer64ID });

        if (operationResult.DataObject is StorageLocation storageLocation)
        {
            operationResult = await dataLayer.DeleteAsync(storageLocation);
            Assert.True(operationResult.IsSuccessStatusCode, "The operation should have been successful.");
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
        }
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all storage locations as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewStorageLocations()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        List<ListView>? storageLocations = await dataLayer.GetAllListViewAsync();

        //Storage location list views must have been returned.
        Assert.NotNull(storageLocations);
        Assert.NotEmpty(storageLocations);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all storage locations for an area asset as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewStorageLocationsWithOwnerId()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, BHSExampleBuilder.MainPartStorageAreaAssetName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to find the area asset");
            return;
        }

        List<ListView>? storageLocations = await dataLayer.GetAllListViewAsync(areaAsset.Integer64ID);

        //Storage location list views must have been returned.
        Assert.NotNull(storageLocations);
        Assert.NotEmpty(storageLocations);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all storage locations from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllStorageLocations()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        List<StorageLocation>? storageLocations = await dataLayer.GetAllAsync();

        //Storage locations must have been returned.
        Assert.NotNull(storageLocations);
        Assert.NotEmpty(storageLocations);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all storage locations for an area asset from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllStorageLocationsWithOwnerId()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, BHSExampleBuilder.MainPartStorageAreaAssetName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to find the area asset");
            return;
        }

        List<StorageLocation>? storageLocations = await dataLayer.GetAllAsync(areaAsset.Integer64ID);

        //Storage locations must have been returned.
        Assert.NotNull(storageLocations);
        Assert.NotEmpty(storageLocations);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request the first storage location from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSingleStorageLocation()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        StorageLocation? storageLocation = await dataLayer.GetSingleAsync();
        Assert.NotNull(storageLocation);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a storage location from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSingleStorageLocationWithIdA()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Get Single Storage Location Test", Name = "Test", OwnerInteger64ID = areaAsset.Integer64ID });

        if (operationResult.DataObject is StorageLocation createdStorageLocation)
        {
            StorageLocation? fetchedStorageLocation = await dataLayer.GetSingleAsync(createdStorageLocation.Integer64ID);
            Assert.NotNull(fetchedStorageLocation);
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
        }
    }

    /// <summary>
    /// The method verifies the server will return a failure if the storage location already exists when adding a new asset.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Duplicate Storage Location Test A", "", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "Duplicate Storage Location Test C")]
    public async Task VerifyUpdateDuplicateStorageLocationFailure(string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation firstStorageLocation = new()
        {
            LocationA = locationA + "1",
            LocationB = locationB + "2",
            LocationC = locationC + "3",
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        firstStorageLocation.Name = firstStorageLocation.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(firstStorageLocation);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first storage location.");
            return;
        }

        StorageLocation secondStorageLocation = new()
        {
            LocationA = locationA + "4",
            LocationB = locationB + "5",
            LocationC = locationC + "6",
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        secondStorageLocation.Name = secondStorageLocation.FriendlyName;
        operationResult = await dataLayer.CreateAsync(secondStorageLocation);

        if (operationResult.DataObject is StorageLocation duplicateStorageLocation)
        {
            duplicateStorageLocation.LocationA = firstStorageLocation.LocationA;
            duplicateStorageLocation.LocationB = firstStorageLocation.LocationB;
            duplicateStorageLocation.LocationC = firstStorageLocation.LocationC;
            operationResult = await dataLayer.UpdateAsync(duplicateStorageLocation);

            //The operation must have failed.
            Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

            //No storage location was returned.
            Assert.Null(operationResult.DataObject);

            //A bad request status was returned.
            Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

            //A validation error was returned.
            Assert.NotNull(operationResult.ServerSideValidationResult);
            Assert.Single(operationResult.ServerSideValidationResult.Errors);

            //The correct error was returned.
            Assert.Contains("location already exists", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
            Assert.Equal(nameof(StorageLocation.LocationA), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
        }
        else
        {
            Assert.Fail("Failed to create the second storage location.");
            return;
        }
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a storage location to be updated by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Location 1", "Test Location X", "", "")]
    [InlineData("Test Location 2", "Test Location X", "Test Location Y", "")]
    [InlineData("Test Location 3", "Test Location X", "Test Location Y", "Test Location Z")]
    public async Task VerifyUpdateStorageLocation(string originalLocationA, string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation originalStorageLocation = new()
        {
            LocationA = originalLocationA,
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        originalStorageLocation.Name = originalStorageLocation.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(originalStorageLocation);

        if (operationResult.IsSuccessStatusCode && operationResult.DataObject is StorageLocation createdStorageLocation)
        {
            StorageLocation updatedStorageLocation = new(createdStorageLocation)
            {
                LocationA = locationA,
                LocationB = locationB,
                LocationC = locationC,
            };
            operationResult = await dataLayer.UpdateAsync(updatedStorageLocation);

            Assert.True(operationResult.IsSuccessStatusCode); //The operation must have been successful.
            Assert.IsType<StorageLocation>(operationResult.DataObject); //A storage location must have been returned.
            Assert.True(new StorageLocationEqualityComparer(true, true, true).Equals((StorageLocation)operationResult.DataObject, updatedStorageLocation), "The data object sent should be the same as the data object returned."); //Original and return must be equal.
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
        }
    }

    /// <summary>
    /// The method verifies the server will return a failure if the area asset doesn't exists when updating a storage location.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyUpdateStorageLocationDependenciesNotExists()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation? storageLocation = new()
        {
            LocationA = "Update Dependencies Not Exists Storage Location Test",
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(storageLocation);
        storageLocation = operationResult.DataObject as StorageLocation;

        if (storageLocation == null)
        {
            Assert.Fail("Failed to create the storage location.");
            return;
        }

        storageLocation.OwnerInteger64ID = 0;
        operationResult = await dataLayer.UpdateAsync(storageLocation);

        //The operation must have failed.
        Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed.");

        //No storage location was returned.
        Assert.Null(operationResult.DataObject);

        //A bad request status was returned.
        Assert.Equal(HttpStatusCode.BadRequest, operationResult.StatusCode);

        //A validation error was returned.
        Assert.NotNull(operationResult.ServerSideValidationResult);
        Assert.Single(operationResult.ServerSideValidationResult.Errors);

        //The correct error was returned.
        Assert.Contains("asset was not found", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
        Assert.Equal(nameof(StorageLocation.OwnerInteger64ID), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
    }

    /// <summary>
    /// The method verifies the server will return a failure if the storage location being updated is old.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyUpdateStorageLocationOldDataConflict()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation()
        {
            LocationA = "Old Data Storage Location Test",
            Name = "Test",
            OwnerInteger64ID = areaAsset.Integer64ID,
        });

        if (operationResult.DataObject is StorageLocation firstStorageLocation)
        {
            StorageLocation secondStorageLocation = new(firstStorageLocation);

            firstStorageLocation.LocationB = "Old Data Storage Location Test 1";
            secondStorageLocation.LocationB = "Old Data Storage Location Test 2";

            operationResult = await dataLayer.UpdateAsync(secondStorageLocation);

            if (!operationResult.IsSuccessStatusCode)
            {
                Assert.Fail("Failed to update the second storage location.");
                return;
            }

            operationResult = await dataLayer.UpdateAsync(firstStorageLocation);

            Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed."); //The operation must have failed.
            Assert.Null(operationResult.DataObject); //No asset was returned.
            Assert.Equal(HttpStatusCode.Conflict, operationResult.StatusCode); //A conflict status was returned.
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
            return;
        }
    }
}
