using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Part;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Part;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TestProject.Test.WebRequest.Part;

/// <summary>
/// The class manages tests for parts using both the http client and server.
/// </summary>
public class PartUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// 
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    public PartUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task AddPartAsync()
    {
#warning Because I'm using a memory database, I cannot confirm 
        HttpClient client = _factory.CreateClient();
        PartDataLayer dataLayer = new(client);

        PartDataObject originalDataObject = new()
        {
            Name = "",
            Description = "",
        };
        OperationResult operationResult = await dataLayer.CreateAsync(originalDataObject);

        Assert.True
        (
            operationResult.IsSuccessStatusCode
            && operationResult.DataObject is PartDataObject returnedDataObject
            && returnedDataObject.Integer64ID > 0
            && returnedDataObject.Name == originalDataObject.Name && returnedDataObject.Description == originalDataObject.Description
        );
    }
}
