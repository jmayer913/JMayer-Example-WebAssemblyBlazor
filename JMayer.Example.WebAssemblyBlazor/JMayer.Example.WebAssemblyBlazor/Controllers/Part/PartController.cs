using JMayer.Example.WebAssemblyBlazor.Shared.Data.Part;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Part;
using JMayer.Web.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.WebAssemblyBlazor.Controllers.Part;

/// <summary>
/// The class manages HTTP requests for CRUD operations associated with a part in a database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PartController : StandardCRUDController<PartDataObject, PartDataLayer>
{
    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="dataLayer">The data layer the controller will interact with.</param>
    /// <param name="logger">The logger the controller will interact with.</param>
    public PartController(PartDataLayer dataLayer, ILogger<PartController> logger) : base(dataLayer, logger) { }
}
