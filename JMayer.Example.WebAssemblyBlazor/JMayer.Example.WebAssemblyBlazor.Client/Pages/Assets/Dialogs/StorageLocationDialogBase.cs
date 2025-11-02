using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Assets.Dialogs;

/// <summary>
/// The class manages user interactions with the StorageLocationDialog.razor dialog.
/// </summary>
public class StorageLocationDialogBase : CardDialogBase<StorageLocation, IStorageLocationDataLayer>
{
    /// <inheritdoc/>
    protected override async Task OnSubmitEditFormAsync()
    {
        //The Name property is used for the ListView and
        //this ensures the ListView displays the friendly
        //name.
        DataObject.Name = DataObject.FriendlyName;
        await base.OnSubmitEditFormAsync();
    }
}
