using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using Microsoft.AspNetCore.Mvc.Testing;

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
            && new AssetEqualityComparer(true).Equals(returnedDataObject, originalDataObject) //Original and return must be equal.
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
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Duplicate Asset Test" });

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No asset was returned.
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
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Test Child 1", ParentID = dataObject.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the child asset.");
        }

        operationResult = await dataLayer.CreateAsync(new Asset() { Name = "Test Child 2", ParentID = dataObject.Integer64ID });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the child asset.");
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
}
