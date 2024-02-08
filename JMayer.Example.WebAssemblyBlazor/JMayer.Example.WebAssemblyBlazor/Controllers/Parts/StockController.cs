using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;
using JMayer.Web.Mvc.Controller;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.WebAssemblyBlazor.Controllers.Parts;

/// <summary>
/// The class manages HTTP requests for CRUD operations associated with a part stock in a database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StockController : SubUserEditableController<Stock, IStockDataLayer>
{
    /// <inheritdoc/>
    public StockController(IStockDataLayer dataLayer, ILogger<StockController> logger) : base(dataLayer, logger) { }
}
