using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;
using JMayer.Web.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.WebAssemblyBlazor.Controllers.Parts;

/// <summary>
/// The class manages HTTP requests for CRUD operations associated with a part in a database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PartController : StandardCRUDController<Part, IPartDataLayer>
{
    /// <inheritdoc/>
    public PartController(IPartDataLayer dataLayer, ILogger<PartController> logger) : base(dataLayer, logger) { }

    /// <summary>
    /// The method returns the categories for the parts.
    /// </summary>
    /// <returns>A list of categories.</returns>
    [HttpGet("Category/All")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        try
        {
            List<Part> dataObjects = await DataLayer.GetAllAsync();
            List<string?> categories = dataObjects.Select(obj => obj.Category).Distinct().ToList();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to return the part categories.");
            return Problem();
        }
    }
}
