using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Dialogs;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts;

/// <summary>
/// The class manages user interactions with the PartSearch.razor page.
/// </summary>
public class PartSearchBase : SearchBase<Part, IPartDataLayer, NewPartDialog>
{
}
