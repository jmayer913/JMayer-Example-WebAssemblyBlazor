using JMayer.Data.Database.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;
using JMayer.Web.Mvc.Controller;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.WebAssemblyBlazor.Controllers.Assets;

/// <summary>
/// The class manages HTTP requests for CRUD operations associated with an asset in a database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AssetController : UserEditableController<Asset, IAssetDataLayer>
{
    /// <inheritdoc/>
    public AssetController(IUserEditableDataLayer<Asset> dataLayer, ILogger logger) : base(dataLayer, logger) { }

    /// <summary>
    /// The method returns the categories for the assets.
    /// </summary>
    /// <returns>A list of categories.</returns>
    [HttpGet("Category/All")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        try
        {
            List<Asset> dataObjects = await DataLayer.GetAllAsync();
            List<string?> categories = dataObjects.Select(obj => obj.Category).Distinct().ToList();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to return the asset categories.");
            return Problem();
        }
    }
}
