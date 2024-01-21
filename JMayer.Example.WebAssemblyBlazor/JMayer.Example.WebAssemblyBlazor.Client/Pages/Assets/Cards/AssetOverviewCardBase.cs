using JMayer.Data.Data;
using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Assets.Cards;

/// <summary>
/// The class manages user interactions with the AssetOverviewCard.razor component.
/// </summary>
public class AssetOverviewCardBase : OverviewCardBase<Asset, IAssetDataLayer>
{
    /// <summary>
    /// The selected asset parent.
    /// </summary>
    private ListView? _selectedAssetParent;

    /// <summary>
    /// The property gets/sets the assets to choose for a parent.
    /// </summary>
    protected List<ListView> Assets { get; set; } = [];

    /// <summary>
    /// The property gets/sets the categories for the parts.
    /// </summary>
    protected List<string> Categories { get; set; } = [];

    /// <summary>
    /// The property gets/sets the parent asset which the user selected.
    /// </summary>
    protected ListView? SelectedAssetParent
    {
        get => _selectedAssetParent;
        set
        {
            _selectedAssetParent = value;
            //Map any selection changes to the ParentID property.
            DataObject.ParentID = value?.Integer64ID ?? 0;
        }
    }

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        Assets = await DataLayer.GetAllListViewAsync() ?? [];
        Categories = await DataLayer.GetCategoriesAsync() ?? [];

        if (DataObject.ParentID != null)
        {
            Asset? parent = await DataLayer.GetSingleAsync($"{DataObject.ParentID}");

            if (parent != null)
            {
                SelectedAssetParent = new ListView()
                {
                    Integer64ID = parent.Integer64ID,
                    Name = parent.Name,
                };
            }
        }
        
        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// The method returns the list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<ListView>> OnAssetParentAutoCompleteSearchAsync(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return await Task.FromResult(Assets);
        }
        else
        {
            return await Task.FromResult(Assets.Where(obj => obj.Name.Contains(value)));
        }
    }

    /// <summary>
    /// The method returns the list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<string>> OnCategoryAutoCompleteSearchAsync(string value)
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
