using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Cards;

/// <summary>
/// The class manages user interactions with the OverviewCard.razor component.
/// </summary>
public class OverviewCardBase : Components.Base.OverviewCardBase<Part, IPartDataLayer>
{
    /// <summary>
    /// The property gets/sets the categories for the parts.
    /// </summary>
    protected List<string> Categories { get; set; } = [];

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        Categories = await DataLayer.GetCategoriesAsync() ?? [];
        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// The method returns the list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <param name="cancellationToken">Used to cancel the task.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<string>> OnCategoryAutoCompleteSearch(string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return await Task.FromResult(Categories);
        }
        else
        {
            return await Task.FromResult(Categories.Where(s => s.Contains(value)));
        }
    }
}
