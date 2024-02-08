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
public class StorageLocationController : SubUserEditableController<StorageLocation, IStorageLocationDataLayer>
{
    /// <inheritdoc/>
    public StorageLocationController(IStorageLocationDataLayer dataLayer, ILogger<StorageLocationController> logger) : base(dataLayer, logger) { }
}
