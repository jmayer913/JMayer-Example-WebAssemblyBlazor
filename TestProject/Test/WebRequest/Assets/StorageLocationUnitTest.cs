using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
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
    /// The method confirms the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CountStorageLocationsAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        long count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method confirms the server will return a failure if the storage location already exists when adding a new asset.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Duplicate Storage Location Test A", "", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "Duplicate Storage Location Test C")]
    public async Task AddDuplicateStorageLocationAsync(string locationA, string locationB, string locationC)
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

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No storage location was returned.
            && operationResult.StatusCode == HttpStatusCode.BadRequest //A bad request status was returned.
            && operationResult.ServerSideValidationResult != null //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors.Count == 1 //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage.Contains("location already exists") //The correct error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].PropertyName == nameof(StorageLocation.LocationA) //The correct error was returned.
        );
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a storage location to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Location A", "", "")]
    [InlineData("Test Location A", "Test Location B", "")]
    [InlineData("Test Location A", "Test Location B", "Test Location C")]
    public async Task AddStorageLocationAsync(string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
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
            && new StorageLocationEqualityComparer(true, true, true).Equals(returnedDataObject, originalDataObject) //Original and return must be equal.
        );
    }

    /// <summary>
    /// The method confirms the server will return a failure if the area asset doesn't exists when adding a new storage location.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task AddStorageLocationDependenciesNotExistsAsync()
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Add Dependencies Not Exists Storage Location Test", Name = "Dependencies Not Exists Storage Location Test", OwnerInteger64ID = 0 });

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No storage location was returned.
            && operationResult.StatusCode == HttpStatusCode.BadRequest //A bad request status was returned.
            && operationResult.ServerSideValidationResult != null //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors.Count == 1 //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage.Contains("asset was not found") //The correct error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].PropertyName == nameof(StorageLocation.OwnerInteger64ID) //The correct error was returned.
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

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Delete Storage Location Test", Name = "Test", OwnerInteger64ID = areaAsset.Integer64ID });

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

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, BHSExampleBuilder.MainPartStorageAreaAssetName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to find the area asset");
            return;
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

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, BHSExampleBuilder.MainPartStorageAreaAssetName);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to find the area asset");
            return;
        }

        List<StorageLocation>? dataObjects = await dataLayer.GetAllAsync(areaAsset.Integer64ID);
        Assert.True(dataObjects != null && dataObjects.Count > 0);
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

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        OperationResult operationResult = await dataLayer.CreateAsync(new StorageLocation() { LocationA = "Get Single Storage Location Test", Name = "Test", OwnerInteger64ID = areaAsset.Integer64ID });

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

    /// <summary>
    /// The method confirms the server will return a failure if the storage location already exists when adding a new asset.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Duplicate Storage Location Test A", "", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "")]
    [InlineData("Duplicate Storage Location Test A", "Duplicate Storage Location Test B", "Duplicate Storage Location Test C")]
    public async Task UpdateDuplicateStorageLocationAsync(string locationA, string locationB, string locationC)
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
        StorageLocation? duplicateStorageLocation = operationResult.DataObject as StorageLocation;

        if (duplicateStorageLocation == null)
        {
            Assert.Fail("Failed to create the second storage location.");
            return;
        }

        duplicateStorageLocation.LocationA = firstStorageLocation.LocationA;
        duplicateStorageLocation.LocationB = firstStorageLocation.LocationB;
        duplicateStorageLocation.LocationC = firstStorageLocation.LocationC;
        operationResult = await dataLayer.UpdateAsync(duplicateStorageLocation);

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No storage location was returned.
            && operationResult.StatusCode == HttpStatusCode.BadRequest //A bad request status was returned.
            && operationResult.ServerSideValidationResult != null //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors.Count == 1 //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage.Contains("location already exists") //The correct error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].PropertyName == nameof(StorageLocation.LocationA) //The correct error was returned.
        );
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a storage location to be updated by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="locationA">The name of the A location.</param>
    /// <param name="locationB">The name of the B location.</param>
    /// <param name="locationC">The name of the C location.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Location 1", "Test Location X", "", "")]
    [InlineData("Test Location 2", "Test Location X", "Test Location Y", "")]
    [InlineData("Test Location 3", "Test Location X", "Test Location Y", "Test Location Z")]
    public async Task UpdateStorageLocationAsync(string originalLocationA, string locationA, string locationB, string locationC)
    {
        HttpClient client = _factory.CreateClient();
        StorageLocationDataLayer dataLayer = new(client);

        Asset? areaAsset = await DataHelper.GetOrCreateAreaAssetAsync(client, Constants.TestAreaAsset);

        if (areaAsset == null)
        {
            Assert.Fail("Failed to retrieve or create the area asset.");
            return;
        }

        StorageLocation originalDataObject = new()
        {
            LocationA = originalLocationA,
            OwnerInteger64ID = areaAsset.Integer64ID,
        };
        originalDataObject.Name = originalDataObject.FriendlyName;
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        if (operationResult.IsSuccessStatusCode && operationResult.DataObject is StorageLocation createdDataObject)
        {
            StorageLocation updatedDataObject = new(createdDataObject)
            {
                LocationA = locationA,
                LocationB = locationB,
                LocationC = locationC,
            };
            operationResult = await dataLayer.UpdateAsync(updatedDataObject);

            Assert.True
            (
                operationResult.IsSuccessStatusCode //The operation must have been successful.
                && operationResult.DataObject is StorageLocation returnedDataObject //A storage location must have been returned.
                && new StorageLocationEqualityComparer(false, false, true).Equals(returnedDataObject, updatedDataObject) //The original data matches the returned data.
            );
        }
        else
        {
            Assert.Fail("Failed to create the storage location.");
        }
    }

    /// <summary>
    /// The method confirms the server will return a failure if the area asset doesn't exists when updating a storage location.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateStorageLocationDependenciesNotExistsAsync()
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

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No storage location was returned.
            && operationResult.StatusCode == HttpStatusCode.BadRequest //A bad request status was returned.
            && operationResult.ServerSideValidationResult != null //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors.Count == 1 //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage.Contains("asset was not found") //The correct error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].PropertyName == nameof(StorageLocation.OwnerInteger64ID) //The correct error was returned.
        );
    }

    /// <summary>
    /// The method confirms the server will return a failure if the storage location being updated is old.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateStorageLocationOldDataAsync()
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
        StorageLocation? firstDataObject = operationResult.DataObject as StorageLocation;

        if (firstDataObject == null)
        {
            Assert.Fail("Failed to create the storage location.");
            return;
        }

        StorageLocation secondDataObject = new(firstDataObject);

        firstDataObject.LocationB = "Old Data Storage Location Test 1";
        secondDataObject.LocationB = "Old Data Storage Location Test 2";

        operationResult = await dataLayer.UpdateAsync(secondDataObject);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to update the second storage location.");
            return;
        }

        operationResult = await dataLayer.UpdateAsync(firstDataObject);

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No storage location was returned.
            && operationResult.StatusCode == HttpStatusCode.Conflict //A conflict status was returned.
        );
    }
}
