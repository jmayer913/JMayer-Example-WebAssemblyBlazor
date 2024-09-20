using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Mvc.Testing;
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
    /// The method verifies the server will return a failure if the part already exists when adding a new part.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyAddDuplicatePartFailure()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Add Duplicate Part Test" });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first part.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Part() { Name = "Add Duplicate Part Test" });

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
        Assert.Contains("name already exists", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
        Assert.Equal(nameof(Part.Name), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a part to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="name">The name of the part.</param>
    /// <param name="description">The description for the part.</param>
    /// <param name="category">The common category for the part.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Test Motor", null, null)]
    [InlineData("Test Push Button", "Test Push Button", null)]
    [InlineData("Test Contact", "Test Contact", "Contact")]
    public async Task VerifyAddPart(string name, string description, string? category)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        Part part = new()
        {
            Category = category,
            Description = description,
            Name = name,
        };
        OperationResult operationResult = await dataLayer.CreateAsync(part);

        Assert.True(operationResult.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.IsType<Part>(operationResult.DataObject); //A part must have been returned.
        Assert.True(new PartEqualityComparer(true, true, true).Equals((Part)operationResult.DataObject, part), "The data object sent should be the same as the data object returned."); //Original and return must be equal.
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request the count from the server and the server can successfully process the request.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyCountParts()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        long count = await dataLayer.CountAsync();
        Assert.True(count > 0);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a part to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeletePart()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Delete Part Test" });
        
        if (operationResult.DataObject is Part part)
        {
            operationResult = await dataLayer.DeleteAsync(part);
            Assert.True(operationResult.IsSuccessStatusCode);
        }
        else
        {
            Assert.Fail("Failed to create the part.");
        }
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all parts from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllParts()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        List<Part>? parts = await dataLayer.GetAllAsync();

        //Parts must have been returned.
        Assert.NotNull(parts);
        Assert.NotEmpty(parts);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all parts as list views from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewParts()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        List<ListView>? parts = await dataLayer.GetAllListViewAsync();

        //Part list views must have been returned.
        Assert.NotNull(parts);
        Assert.NotEmpty(parts);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request all part categories from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllPartCategories()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        List<string>? categories = await dataLayer.GetCategoriesAsync();

        //Part categories must have been returned.
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request the first part from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSinglePart()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        Part? part = await dataLayer.GetSingleAsync();
        Assert.NotNull(part);
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a part from the server and the server can successfully process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSinglePartWithId()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Get Single Part Test" });

        if (operationResult.DataObject is Part createdPart)
        {
            Part? fetchedPart = await dataLayer.GetSingleAsync(createdPart.Integer64ID);
            Assert.NotNull(fetchedPart);
        }
        else
        {
            Assert.Fail("Failed to create the part.");
        }
    }

    /// <summary>
    /// The method verifies the server will return a failure if the part already exists when update a part.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyUpdateDuplicatePartFailure()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = "Add Duplicate Part Test 1" });

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("Failed to create the first part.");
            return;
        }

        operationResult = await dataLayer.CreateAsync(new Part() { Name = "Add Duplicate Part Test 2" });

        if (operationResult.DataObject is Part part)
        {
            part.Name = "Add Duplicate Part Test 1";
            operationResult = await dataLayer.UpdateAsync(part);

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
            Assert.Contains("name already exists", operationResult.ServerSideValidationResult.Errors[0].ErrorMessage);
            Assert.Equal(nameof(Part.Name), operationResult.ServerSideValidationResult.Errors[0].PropertyName);
        }
        else
        {
            Assert.Fail("Failed to create the second part.");
            return;
        }
    }

    /// <summary>
    /// The method verifies the HTTP data layer can request a part to be updated by the server and the server can successfully process the request.
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
    public async Task VerifyUpdatePart(string originalName, string newName, string description, string? category, string? make, string? model, string? manufacturer, string? manufacturerNumber, bool obsolete)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part() { Name = originalName });

        if (operationResult.IsSuccessStatusCode && operationResult.DataObject is Part createdPart)
        {
            Part updatedPart = new(createdPart)
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
            operationResult = await dataLayer.UpdateAsync(updatedPart);

            Assert.True(operationResult.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
            Assert.IsType<Part>(operationResult.DataObject); //A part must have been returned.
            Assert.True(new PartEqualityComparer(true, true, true).Equals((Part)operationResult.DataObject, updatedPart), "The data object sent should be the same as the data object returned."); //Original and return must be equal.
        }
        else
        {
            Assert.Fail("Failed to create the part.");
        }
    }

    /// <summary>
    /// The method verifies the server will return a failure if the part being updated is old.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyUpdatePartOldDataConflict()
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        OperationResult operationResult = await dataLayer.CreateAsync(new Part()
        {
            Name = "Old Data Part Test",
        });

        if (operationResult.DataObject is Part firstPart)
        {
            Part secondPart = new(firstPart);

            firstPart.Description = "A description";
            secondPart.Obsolete = true;

            operationResult = await dataLayer.UpdateAsync(secondPart);

            if (!operationResult.IsSuccessStatusCode)
            {
                Assert.Fail("Failed to update the second part.");
                return;
            }

            operationResult = await dataLayer.UpdateAsync(firstPart);

            Assert.False(operationResult.IsSuccessStatusCode, "The operation should have failed."); //The operation must have failed.
            Assert.Null(operationResult.DataObject); //No asset was returned.
            Assert.Equal(HttpStatusCode.Conflict, operationResult.StatusCode); //A conflict status was returned.
        }
        else 
        {
            Assert.Fail("Failed to create the part.");
            return;
        }
    }
}
