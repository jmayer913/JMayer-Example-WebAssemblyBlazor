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
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public PartUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method confirms the HTTP data layer can request a part to be created by the server and the server can process the request.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task AddPartAsync()
    {
#warning This should be turned into a theory with parameters for the Part object and several variations to represent what the user can enter.
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        Part originalDataObject = new()
        {
            Name = "Part1",
            Description = "",
        };
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        if (!operationResult.IsSuccessStatusCode)
        {
            Assert.Fail("A negative response was returned.");
        }
        else if (operationResult.DataObject is Part returnedDataObject)
        {
            Part? databaseDataObject = await dataLayer.GetSingleAsync(returnedDataObject.Integer64ID.ToString() ?? string.Empty);

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
