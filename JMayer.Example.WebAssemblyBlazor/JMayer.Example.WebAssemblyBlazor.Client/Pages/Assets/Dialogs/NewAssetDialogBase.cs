using JMayer.Data.Data;
using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Assets.Dialogs;

/// <summary>
/// The class manages user interactions with the NewAssetDialog.razor dialog.
/// </summary>
public class NewAssetDialogBase : NewDialogBase<Asset, IAssetDataLayer>
{
    /// <summary>
    /// The property gets/sets the categories for the parts.
    /// </summary>
    protected List<string> Categories { get; set; } = [];

    /// <summary>
    /// The property gets/sets the assets to choose for a parent.
    /// </summary>
    protected List<ListView> ParentAssets { get; set; } = [];

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected override async Task OnParametersSetAsync()
    {
        Task<List<string>?> categoryTask = DataLayer.GetCategoriesAsync();
        Task<List<ListView>?> parentAssetTask = DataLayer.GetAllListViewAsync();

        try
        {
            await Task.WhenAll(categoryTask, parentAssetTask);
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }

        Categories = categoryTask.Result ?? [];
        ParentAssets = parentAssetTask.Result ?? [];

        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// The method returns the asset list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <param name="cancellationToken">Used to cancel the task.</param>
    /// <returns>A list of acceptable parent assets.</returns>
    protected async Task<IEnumerable<long?>> OnAssetParentAutoCompleteSearchAsync(string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return await Task.FromResult(ParentAssets.Select(obj => (long?)obj.Integer64ID));
        }
        else
        {
            return await Task.FromResult(ParentAssets.Where(obj => obj.Name.Contains(value)).Select(obj => (long?)obj.Integer64ID));
        }
    }

    /// <summary>
    /// The method returns the category list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <param name="cancellationToken">Used to cancel the task.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<string>> OnCategoryAutoCompleteSearchAsync(string value, CancellationToken cancellationToken)
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
