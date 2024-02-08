using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Dialogs;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Cards;

/// <summary>
/// The class manages user interactions with the StockCard.razor component.
/// </summary>
public class StockCardBase : EditableCardBase<Stock, IStockDataLayer, StockDialog>
{
}
