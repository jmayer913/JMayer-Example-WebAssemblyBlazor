using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TestProject.Test.WebRequest.Parts;

/// <summary>
/// The class manages tests for parts using both the http client and server.
/// </summary>
public class PartUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The constant for the maximum number parts to be mass created.
    /// </summary>
    private const int MaxCreationLimit = 100;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public PartUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be created by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="name">The name of the part.</param>
    /// <param name="description">The description for the part.</param>
    /// <param name="category">The common category for the part.</param>
    /// <param name="make">The make of the part.</param>
    /// <param name="model">The model for the part.</param>
    /// <param name="manufacturer">Who makes the part.</param>
    /// <param name="manufacturerNumber">The identifier the manufacturer uses for the part.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData("Motor", "Motor", null, null, null, null, null)]
    [InlineData("Push Button, Extended Head, with Guard, 12-130V AC/DC Green, 30mm", "AB Push Button", null, null, null, "AB", "800TC-QAH2G")]
    [InlineData("Mini-PS-100 230AC/10-15DC/8", "Phoenix Contact Power Supply", "Power Supply", null, null, "Phoenix Contact", "2866297")]
    public async Task AddPartAsync(string name, string description, string? category, string? make , string? model, string? manufacturer, string? manufacturerNumber)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        Part originalDataObject = new()
        {
            Category = category,
            Description = description,
            Make = make,
            Manufacturer = manufacturer,
            ManufacturerNumber = manufacturerNumber,
            Model = model,
            Name = name,
        };
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("A negative response was returned.");
        }
        else if (operationResult.DataObject is Part returnedDataObject)
        {
            Part? databaseDataObject = await dataLayer.GetSingleAsync(returnedDataObject.Integer64ID);

            if (databaseDataObject == null)
            {
                Assert.Fail("No database object was found.");
            }
            else
            {
                Assert.True(new PartEqualityComparer().Equals(returnedDataObject, databaseDataObject));
            }
        }
        else
        {
            Assert.Fail("No data object was returned.");
        }
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be deleted by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="id">The id to delete.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public async Task DeletePartAsync(int id)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        await MassCreateGenericPartsAsync(dataLayer);

        OperationResult operationResult = await dataLayer.DeleteAsync(new Part() { Integer64ID = id });
        Assert.True(operationResult.IsSuccessStatusCode);
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

        await MassCreateGenericPartsAsync(dataLayer);

        List<Part>? dataObjects = await dataLayer.GetAllAsync();
        Assert.True(dataObjects != null && dataObjects.Count == MaxCreationLimit);
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part from the server and the server can successfully process the request.
    /// </summary>
    /// <param name="id">The id to retrieve.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public async Task GetSinglePartAsync(int id)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        await MassCreateGenericPartsAsync(dataLayer);

        Part? dataObject = await dataLayer.GetSingleAsync(id);
        Assert.NotNull(dataObject);
    }

    /// <summary>
    /// The method mass creates a bunch of generic parts inorder to do certain tests.
    /// </summary>
    /// <param name="dataLayer">The data layer to interact with.</param>
    /// <returns>A Task object for the async.</returns>
    private static async Task MassCreateGenericPartsAsync(PartDataLayer dataLayer)
    {
        for (int index = 0; index < MaxCreationLimit; index++)
        {
            OperationResult operationResult = await dataLayer.CreateAsync(new Part()
            {
                Name = $"Part{index}",
            });
        }
    }

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be updated by the server and the server can successfully process the request.
    /// </summary>
    /// <param name="id">The id to update.</param>
    /// <param name="name">The name of the part.</param>
    /// <param name="description">The description for the part.</param>
    /// <param name="category">The common category for the part.</param>
    /// <param name="make">The make of the part.</param>
    /// <param name="model">The model for the part.</param>
    /// <param name="manufacturer">Who makes the part.</param>
    /// <param name="manufacturerNumber">The identifier the manufacturer uses for the part.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(1, "Motor", "Motor", null, null, null, null, null)]
    [InlineData(25, "Push Button, Extended Head, with Guard, 12-130V AC/DC Green, 30mm", "AB Push Button", null, null, null, "AB", "800TC-QAH2G")]
    [InlineData(50, "Mini-PS-100 230AC/10-15DC/8", "Phoenix Contact Power Supply", "Power Supply", null, null, "Phoenix Contact", "2866297")]
    public async Task UpdatePartAsync(int id, string name, string description, string? category, string? make, string? model, string? manufacturer, string? manufacturerNumber)
    {
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        await MassCreateGenericPartsAsync(dataLayer);

        Part? dataObject = await dataLayer.GetSingleAsync(id);

        if (dataObject == null)
        {
            Assert.Fail("Failed to find the part.");
        }
        else
        {
            dataObject.Name = name;
            dataObject.Description = description;
            dataObject.Category = category;
            dataObject.Make = make;
            dataObject.Manufacturer = manufacturer;
            dataObject.ManufacturerNumber = manufacturerNumber;
            dataObject.Model = model;
            
            OperationResult operationResult = await dataLayer.UpdateAsync(dataObject);

            if (!operationResult.IsSuccessStatusCode)
            {
                Assert.Fail("A negative response was returned.");
            }
            else if (operationResult.DataObject is Part returnedDataObject)
            {
                Part? databaseDataObject = await dataLayer.GetSingleAsync(id);

                if (databaseDataObject == null)
                {
                    Assert.Fail("No database object was found.");
                }
                else
                {
                    Assert.True(new PartEqualityComparer().Equals(returnedDataObject, databaseDataObject));
                }
            }
            else
            {
                Assert.Fail("No data object was returned.");
            }
        }
    }
}
