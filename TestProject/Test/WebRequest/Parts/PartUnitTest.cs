using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TestProject.Test.WebRequest.Parts;

/// <summary>
/// The class manages tests for parts using both the http client and server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class PartUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public PartUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method confirms the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CountPartsAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        int count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="name">The name of the part.</param>
    /// <param name="description">The description for the part.</param>
    /// <param name="category">The common category for the part.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Motor", null, null)]
    [InlineData("Test Push Button", "Test Push Button", null)]
    [InlineData("Test Contact", "Test Contact", "Contact")]
    public async Task AddPartAsync(string name, string description, string? category)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        Part originalDataObject = new()
        {
            Category = category,
            Description = description,
            Name = name,
        };
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        Assert.True
        (
            operationResult.IsSuccessStatusCode //The operation must have been successful.
            && operationResult.DataObject is Part returnedDataObject //A part must have been returned.
            && new PartEqualityComparer(true).Equals(returnedDataObject, originalDataObject)
        );
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeletePartAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Delete Test" });
        
        if (operationResult.DataObject is Part partDataObject)
        {
            operationResult = await dataLayer.DeleteAsync(partDataObject);
            Assert.True(operationResult.IsSuccessStatusCode);
        }
        else
        {
            Assert.Fail("Creating the part for the Delete test failed.");
        }
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all parts from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllPartsAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        List<Part>? dataObjects = await dataLayer.GetAllAsync();
        Assert.True(dataObjects != null && dataObjects.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all parts as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewPartsAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        List<ListView>? listViews = await dataLayer.GetAllListViewAsync();
        Assert.True(listViews != null && listViews.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request all part categories from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllPartCategoriesAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        List<string>? categories = await dataLayer.GetCategoriesAsync();
        Assert.True(categories != null && categories.Count > 0);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request the first part from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSinglePartAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        Part? dataObject = await dataLayer.GetSingleAsync();
        Assert.NotNull(dataObject);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSinglePartWithIdAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Single Test" });

        if (operationResult.DataObject is Part partDataObject)
        {
            Part? dataObject = await dataLayer.GetSingleAsync(partDataObject.Integer64ID);
            Assert.NotNull(dataObject);
        }
        else
        {
            Assert.Fail("Creating the part for the Get Single with an Id test.");
        }
    }

    ///// <summary>
    ///// The method confirms the HTTP data layer can request a part to be updated by the server and the server can successfully process the request.
    ///// </summary>
    ///// <param name="id">The id to update.</param>
    ///// <param name="name">The name of the part.</param>
    ///// <param name="description">The description for the part.</param>
    ///// <param name="category">The common category for the part.</param>
    ///// <param name="make">The make of the part.</param>
    ///// <param name="model">The model for the part.</param>
    ///// <param name="manufacturer">Who makes the part.</param>
    ///// <param name="manufacturerNumber">The identifier the manufacturer uses for the part.</param>
    ///// <returns>A Task object for the async.</returns>
    //[Theory]
    //[InlineData(1, "Motor", "Motor", null, null, null, null, null)]
    //[InlineData(25, "Push Button, Extended Head, with Guard, 12-130V AC/DC Green, 30mm", "AB Push Button", null, null, null, "AB", "800TC-QAH2G")]
    //[InlineData(50, "Mini-PS-100 230AC/10-15DC/8", "Phoenix Contact Power Supply", "Power Supply", null, null, "Phoenix Contact", "2866297")]
    //public async Task UpdatePartAsync(int id, string name, string description, string? category, string? make, string? model, string? manufacturer, string? manufacturerNumber)
    //{
    //    HttpClient client = _factory.CreateClient();
    //    PartDataLayer dataLayer = new(client);

    //    Part? dataObject = await dataLayer.GetSingleAsync(id);

    //    if (dataObject == null)
    //    {
    //        Assert.Fail("Failed to find the part.");
    //    }
    //    else
    //    {
    //        dataObject.Name = name;
    //        dataObject.Description = description;
    //        dataObject.Category = category;
    //        dataObject.Make = make;
    //        dataObject.Manufacturer = manufacturer;
    //        dataObject.ManufacturerNumber = manufacturerNumber;
    //        dataObject.Model = model;
            
    //        OperationResult operationResult = await dataLayer.UpdateAsync(dataObject);

    //        if (!operationResult.IsSuccessStatusCode)
    //        {
    //            Assert.Fail("A negative response was returned.");
    //        }
    //        else if (operationResult.DataObject is Part returnedDataObject)
    //        {
    //            Part? databaseDataObject = await dataLayer.GetSingleAsync(id);

    //            if (databaseDataObject == null)
    //            {
    //                Assert.Fail("No database object was found.");
    //            }
    //            else
    //            {
    //                Assert.True(new PartEqualityComparer().Equals(returnedDataObject, databaseDataObject));
    //            }
    //        }
    //        else
    //        {
    //            Assert.Fail("No data object was returned.");
    //        }
    //    }
    //}
}
