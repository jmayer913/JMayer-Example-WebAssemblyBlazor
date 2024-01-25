using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Database.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;
using JMayer.Web.Mvc.Controller;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.WebAssemblyBlazor.Controllers.Assets;

/// <summary>
/// The class manages HTTP requests for CRUD operations associated with a storage location in a database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StorageLocationController : UserEditableController<StorageLocation, IStorageLocationDataLayer>
{
    /// <inheritdoc/>
    public StorageLocationController(IStorageLocationDataLayer dataLayer, ILogger logger) : base(dataLayer, logger) { }

    /// <summary>
    /// The method returns a page of data objects using the data layer.
    /// </summary>
    /// <param name="assetId">The asset to filter for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <returns>A list of data objects.</returns>
    [HttpGet("Page/{assetId}")]
    public async Task<IActionResult> GetPageAsync(int assetId, [FromQuery] QueryDefinition queryDefinition)
    {
        try
        {
            //Inject a filter for the asset so it always returns storage locations for a specific asset.
            queryDefinition.FilterDefinitions.Insert(0, new FilterDefinition()
            {
                FilterOn = nameof(StorageLocation.AssetId),
                Operator = FilterDefinition.EqualsOperator,
                Value = assetId.ToString(),
            });

            PagedList<StorageLocation> dataObjects = await DataLayer.GetPageAsync(queryDefinition);
            return Ok(dataObjects);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to return a page of {Type} data objects for the {AssetId} asset.", DataObjectTypeName, assetId);
            return Problem();
        }
    }

    /// <summary>
    /// The method returns a page of data objects as list views using the data layer.
    /// </summary>
    /// <param name="assetId">The asset to filter for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <returns>A list of data objects.</returns>
    [HttpGet("Page/ListView/{assetId}")]
    public async Task<IActionResult> GetPageListViewAsync(int assetId, [FromQuery] QueryDefinition queryDefinition)
    {
        try
        {
            //Inject a filter for the asset so it always returns storage locations for a specific asset.
            queryDefinition.FilterDefinitions.Insert(0, new FilterDefinition()
            {
                FilterOn = nameof(StorageLocation.AssetId),
                Operator = FilterDefinition.EqualsOperator,
                Value = assetId.ToString(),
            });

            PagedList<ListView> dataObjects = await ((IUserEditableDataLayer<StorageLocation>)DataLayer).GetPageListViewAsync(queryDefinition);
            return Ok(dataObjects);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to return a page of {Type} data objects as list views for the {AssetId} asset.", DataObjectTypeName, assetId);
            return Problem();
        }
    }
}
