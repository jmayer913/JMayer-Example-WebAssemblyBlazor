using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.CodeAnalysis;
using System.Net;

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
    /// The method confirms the server will return a failure if the part already exists when adding a new part.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task AddDuplicatePartAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Duplicate Part Test" });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first part.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Part() { Name = "Duplicate Part Test" });

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No part was returned.
            && operationResult.StatusCode == HttpStatusCode.BadRequest //A bad request status was returned.
            && operationResult.ServerSideValidationResult != null //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors.Count == 1 //A validation error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage.Contains("name already exists") //The correct error was returned.
            && operationResult.ServerSideValidationResult.Errors[0].PropertyName == nameof(Part.Name) //The correct error was returned.
        );
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
            && new PartEqualityComparer(true, true, true).Equals(returnedDataObject, originalDataObject)
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

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Delete Part Test" });
        
        if (operationResult.DataObject is Part partDataObject)
        {
            operationResult = await dataLayer.DeleteAsync(partDataObject);
            Assert.True(operationResult.IsSuccessStatusCode);
        }
        else
        {
            Assert.Fail("Failed to create the part.");
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

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Get Single Part Test" });

        if (operationResult.DataObject is Part partDataObject)
        {
            Part? dataObject = await dataLayer.GetSingleAsync(partDataObject.Integer64ID);
            Assert.NotNull(dataObject);
        }
        else
        {
            Assert.Fail("Failed to create the part.");
        }
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be updated by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="originalName">The orginal name of the part.</param>
    /// <param name="newName">The new name of the part.</param>
    /// <param name="description">The description for the part.</param>
    /// <param name="category">The common category for the part.</param>
    /// <param name="make">The make of the part.</param>
    /// <param name="model">The model for the part.</param>
    /// <param name="manufacturer">Who makes the part.</param>
    /// <param name="manufacturerNumber">The identifier the manufacturer uses for the part.</param>
    /// <param name="obsolete">Marks the part as obsolete.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Part 1", "Test Encoder", null, null, null, null, null, null, false)]
    [InlineData("Test Part 2", "Test Wire", "Test Wire", null, null, null, null, null, false)]
    [InlineData("Test Part 3", "Test Photoeye", "Test Photoeye", "Photoeye", null, null, null, null, false)]
    [InlineData("Test Part 4", "Test Brake", "Test Brake", "Brake", "Make", null, null, null, false)]
    [InlineData("Test Part 5", "Test PLC", "Test PLC", "PLC", "Make", "Model", null, null, false)]
    [InlineData("Test Part 6", "Test Limit Switch", "Test Limit Switch", "Switch", "Make", "Model", "Manufacturer", null, false)]
    [InlineData("Test Part 7", "Test Belt", "Test Belt", "Belt", "Make", "Model", "Manufacturer", "Manufacturer Number", false)]
    [InlineData("Test Part 8", "Test VFD", "Test VFD", "VFD", "Make", "Model", "Manufacturer", "Manufacturer Number", true)]
    public async Task UpdatePartAsync(string originalName, string newName, string description, string? category, string? make, string? model, string? manufacturer, string? manufacturerNumber, bool obsolete)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = originalName });

        if (operationResult.IsSuccessStatusCode && operationResult.DataObject is Part createdDataObject)
        {
            Part updatedDataObject = new(createdDataObject)
            {
                Category = category,
                Description = description,
                Make = make,
                Manufacturer = manufacturer,
                ManufacturerNumber = manufacturerNumber,
                Model = model,
                Name = newName,
                Obsolete = obsolete,
            };
            operationResult = await dataLayer.UpdateAsync(updatedDataObject);

            Assert.True
            (
                operationResult.IsSuccessStatusCode //The operation must have been successful.
                && operationResult.DataObject is Part returnedDataObject //A part must have been returned.
                && new PartEqualityComparer(false, false, true).Equals(returnedDataObject, updatedDataObject) //The original data matches the returned data.
            );
        }
        else
        {
            Assert.Fail("Failed to create the part.");
        }
    }

    /// <summary>
    /// The method confirms the server will return a failure if the part being updated is old.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdatePartOldDataAsync()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part()
        {
            Name = "Old Data Part Test",
        });
        Part? firstDataObject = operationResult.DataObject as Part;

        if (firstDataObject == null)
        {
            Assert.Fail("Failed to create the part.");
            return;
        }

        Part secondDataObject = new(firstDataObject);

        firstDataObject.Description = "A description";
        secondDataObject.Obsolete = true;

        operationResult = await dataLayer.UpdateAsync(secondDataObject);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to update the second part.");
            return;
        }

        operationResult = await dataLayer.UpdateAsync(firstDataObject);

        Assert.True
        (
            !operationResult.IsSuccessStatusCode //The operation must have failed.
            && operationResult.DataObject == null //No part was returned.
            && operationResult.StatusCode == HttpStatusCode.Conflict //A conflict status was returned.
        );
    }
}
