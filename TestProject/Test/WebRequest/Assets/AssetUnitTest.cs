using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace TestProject.Test.WebRequest.Assets;

/// <summary>
/// The class manages tests for assets using both the http client and server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class AssetUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public AssetUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method confirms the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CountAssetsAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        int count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request an asset to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="name">The name of the asset.</param>
    /// <param name="description">The description for the asset.</param>
    /// <param name="category">The common category for the asset.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Area 1", null, AssetType.Area, null, null)]
    [InlineData("Test Area 2", "Test Area 2", AssetType.Area, null, null)]
    [InlineData("Test Area 3", "Test Area 3", AssetType.Area, "Main Bag Room", null)]
    [InlineData("Test Area 4", "Test Area 4", AssetType.Area, "Main Bag Room", "Area")]
    [InlineData("Test Group 1", null, AssetType.Group, null, null)]
    [InlineData("Test Group 2", "Test Group 2", AssetType.Group, null, null)]
    [InlineData("Test Group 3", "Test Group 3", AssetType.Group, "Main Bag Room", null)]
    [InlineData("Test Group 4", "Test Group 4", AssetType.Group, "Main Bag Room", "Group")]
    [InlineData("Test Equipment 1", null, AssetType.Equipment, null, null)]
    [InlineData("Test Equipment 2", "Test Equipment 2", AssetType.Equipment, null, null)]
    [InlineData("Test Equipment 3", "Test Equipment 3", AssetType.Equipment, "Main Bag Room", null)]
    [InlineData("Test Equipment 4", "Test Equipment 4", AssetType.Equipment, "Main Bag Room", "Equipment")]
    public async Task AddAssetAsync(string name, string description, AssetType assetType, string parentName, string? category)
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        long? parentID = null;

        if (!string.IsNullOrEmpty(parentName))
        {
            parentID = (await dataLayer.GetAllListViewAsync())?.FirstOrDefault(obj => obj.Name == parentName)?.Integer64ID;

            if (parentID == null)
            {
                Assert.Fail("The parent asset was not found.");
                return;
            }
        }

        Asset originalDataObject = new()
        {
            Category = category,
            Description = description,
            Name = name,
            ParentID = parentID,
            Type = assetType,
        };
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        Assert.True
        (
            operationResult.IsSuccessStatusCode //The operation must have been successful.
            && operationResult.DataObject is Asset returnedDataObject //An asset must have been returned.
            && new AssetEqualityComparer(true, true, true).Equals(returnedDataObject, originalDataObject) //Original and return must be equal.
        );
    }

    /// <summary>
    /// The method confirms the server will return a failure if the asset already exists when adding a new asset.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task AddDuplicateAssetAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Duplicate Asset Test" });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Duplicate Asset Test" });

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No asset was returned.
            && operationResult.StatusCode == HttpStatusCode.BadRequest //A bad request status was returned.
            && operationResult.ServerSideValidationResult != null //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors.Count == 1 //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage.Contains("name already exists") //The correct error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].PropertyName == nameof(Asset.Name) //The correct error was returned.
        );
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request an asset to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteAssetAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Delete Asset Test" });

        if (operationResult.DataObject is Asset dataObject)
        {
            operationResult = await dataLayer.DeleteAsync(dataObject);
            Assert.True(operationResult.IsSuccessStatusCode);
        }
        else
        {
            Assert.Fail("Failed to create the asset.");
        }
    }

    /// <summary>
    /// The method confirms on the server-side if a parent asset is deleted, the children are also deleted.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteAssetParentAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        int preCount = await dataLayer.CountAsync();
        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Delete Parent Test" });
        Asset? dataObject = operationResult.DataObject as Asset;

        if (dataObject == null)
        {
            Assert.Fail("Failed to create the parent asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Test Child 1", ParentID = dataObject.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the child asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Test Child 2", ParentID = dataObject.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the child asset.");
            return;
        }

        operationResult = await dataLayer.DeleteAsync(dataObject);
        int postCount = await dataLayer.CountAsync();

        Assert.True(operationResult.IsSuccessStatusCode && preCount == postCount);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all assets from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAssetsAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        List<Asset>? dataObjects = await dataLayer.GetAllAsync();
        Assert.True(dataObjects != null && dataObjects.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all assets as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewAssetsAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        List<ListView>? listViews = await dataLayer.GetAllListViewAsync();
        Assert.True(listViews != null && listViews.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all asset categories from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAssetCategoriesAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        List<string>? categories = await dataLayer.GetCategoriesAsync();
        Assert.True(categories != null && categories.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request the first asset from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleAssetAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        Asset? dataObject = await dataLayer.GetSingleAsync();
        Assert.NotNull(dataObject);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request an asset from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleAssetWithIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Get Single Asset Test" });

        if (operationResult.DataObject is Asset assetDataObject)
        {
            Asset? dataObject = await dataLayer.GetSingleAsync(assetDataObject.Integer64ID);
            Assert.NotNull(dataObject);
        }
        else
        {
            Assert.Fail("Failed to create the asset.");
        }
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request an asset to be updated by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="originalName">The orginal name of the asset.</param>
    /// <param name="newName">The new name of the asset.</param>
    /// <param name="description">The description for the asset.</param>
    /// <param name="category">The common category for the asset.</param>
    /// <param name="make">The make of the asset.</param>
    /// <param name="model">The model for the asset.</param>
    /// <param name="manufacturer">Who makes the asset.</param>
    /// <param name="manufacturerNumber">The identifier the manufacturer uses for the asset.</param>
    /// <param name="online">Marks the asset as obsolete.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Asset 1", "Test AL1", null, null, null, null, null, null, Priority.Low, false)]
    [InlineData("Test Asset 2", "Test AL1-01", "Test AL1-02", null, null, null, null, null, Priority.Medium, false)]
    [InlineData("Test Asset 3", "Test AL1-02", "Test AL1-02", "Conveyor", null, null, null, null, Priority.High, false)]
    [InlineData("Test Asset 4", "Test AL1-03", "Test AL1-03", "Conveyor", "Make", null, null, null, Priority.Medium, false)]
    [InlineData("Test Asset 5", "Test AL1-04", "Test AL1-04", "Conveyor", "Make", "Model", null, null, Priority.Medium, false)]
    [InlineData("Test Asset 6", "Test AL1-05", "Test AL1-05", "Conveyor", "Make", "Model", "Manufacturer", null, Priority.Medium, false)]
    [InlineData("Test Asset 7", "Test AL1-05-BSD", "Test AL1-05-BSD", "BSD", "Make", "Model", "Manufacturer", "Manufacturer Number", Priority.Medium, false)]
    [InlineData("Test Asset 8", "Test AL1-VSU", "Test AL1-VSU", "VSU", "Make", "Model", "Manufacturer", "Manufacturer Number", Priority.Medium, true)]
    public async Task UpdateAssetAsync(string originalName, string newName, string description, string? category, string? make, string? model, string? manufacturer, string? manufacturerNumber, Priority priority, bool online)
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = originalName });

        if (operationResult.IsSuccessStatusCode && operationResult.DataObject is Asset createdDataObject)
        {
            Asset updatedDataObject = new(createdDataObject)
            {
                Category = category,
                Description = description,
                IsOnline = online,
                Make = make,
                Manufacturer = manufacturer,
                ManufacturerNumber = manufacturerNumber,
                Model = model,
                Name = newName,
                Priority = priority,
                Type = AssetType.Equipment,
            };
            operationResult = await dataLayer.UpdateAsync(updatedDataObject);

            Assert.True
            (
                operationResult.IsSuccessStatusCode //The operation must have been successful.
                && operationResult.DataObject is Asset returnedDataObject //An asset must have been returned.
                && new AssetEqualityComparer(false, false, true).Equals(returnedDataObject, updatedDataObject) //The original data matches the returned data.
            );
        }
        else
        {
            Assert.Fail("Failed to create the asset.");
        }
    }

    /// <summary>
    /// The method confirms the server will return a failure if the asset being updated is old.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateAssetOldDataAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset()
        {
            Name = "Old Data Asset Test",
        });
        Asset? firstDataObject = operationResult.DataObject as Asset;

        if (firstDataObject == null)
        {
            Assert.Fail("Failed to create the asset.");
            return;
        }

        Asset secondDataObject = new(firstDataObject);

        firstDataObject.Description = "A description";
        secondDataObject.Category = "A Category";

        operationResult = await dataLayer.UpdateAsync(secondDataObject);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to update the second asset.");
            return;
        }

        operationResult = await dataLayer.UpdateAsync(firstDataObject);

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No asset was returned.
            && operationResult.StatusCode == HttpStatusCode.Conflict //A conflict status was returned.
        );
    }

    /// <summary>
    /// The method confirms the parent path is updated when the root asset is changed to a different asset.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateAssetTreeStructureAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Root Asset Tree Structure Test" });
        Asset? rootAsset = operationResult.DataObject as Asset;

        if (rootAsset == null)
        {
            Assert.Fail("Failed to create the root asset");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Other Root Asset Tree Structure Test" });
        Asset? otherRootAsset = operationResult.DataObject as Asset;

        if (otherRootAsset == null)
        {
            Assert.Fail("Failed to create the other root asset");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Middle Asset Tree Structure Test", ParentID = rootAsset.Integer64ID });
        Asset? middleAsset = operationResult.DataObject as Asset;

        if (middleAsset == null)
        {
            Assert.Fail("Failed to create the middle asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Leaf Asset Tree Structure Test 1", ParentID = middleAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first leaf asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Leaf Asset Tree Structure Test 2", ParentID = middleAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the second leaf asset.");
            return;
        }

        middleAsset.ParentID = otherRootAsset.Integer64ID;
        operationResult = await dataLayer.UpdateAsync(middleAsset);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to update the parent of the middle asset.");
            return;
        }

        List<Asset>? allAssets = await dataLayer.GetAllAsync();

        if (allAssets == null)
        {
            Assert.Fail("Failed to retrieve the assets.");
            return;
        }

        List<Asset> testTreeAssets = allAssets.Where(obj => obj.Integer64ID == middleAsset.Integer64ID || obj.ParentID == middleAsset.Integer64ID).ToList();
        Assert.True(testTreeAssets.All(obj => obj.ParentPath != null && obj.ParentPath.Contains(otherRootAsset.Name)));
    }

    /// <summary>
    /// The method confirms the parent path is updated when the root asset is renamed.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateRootAssetNameAsync()
    {
        HttpClient client = _factory.CreateClient();
        AssetDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Root Asset Rename Test" });
        Asset? rootAsset = operationResult.DataObject as Asset;

        if (rootAsset == null)
        {
            Assert.Fail("Failed to create the root asset");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Middle Asset Rename Test", ParentID = rootAsset.Integer64ID });
        Asset? middleAsset = operationResult.DataObject as Asset;

        if (middleAsset == null)
        {
            Assert.Fail("Failed to create the middle asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Leaf Asset Rename Test 1", ParentID = middleAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first leaf asset.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Leaf Asset Rename Test 2", ParentID = middleAsset.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the second leaf asset.");
            return;
        }

        rootAsset.Name = "New Root Asset Rename Test";
        operationResult = await dataLayer.UpdateAsync(rootAsset);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to rename of the root asset.");
            return;
        }

        List<Asset>? allAssets = await dataLayer.GetAllAsync();

        if (allAssets == null)
        {
            Assert.Fail("Failed to retrieve the assets.");
            return;
        }

        List<Asset> testTreeAssets = allAssets.Where(obj => obj.Integer64ID == middleAsset.Integer64ID || obj.ParentID ==  middleAsset.Integer64ID).ToList();
        Assert.True(testTreeAssets.All(obj => obj.ParentPath != null && obj.ParentPath.Contains(rootAsset.Name)));
    }
}
