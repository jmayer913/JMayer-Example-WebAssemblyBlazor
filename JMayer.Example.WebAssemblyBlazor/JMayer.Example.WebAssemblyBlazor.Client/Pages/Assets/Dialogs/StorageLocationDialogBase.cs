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
    /// <remarks>Overridden because the Name property is used for the ListView and we need to make sure the Name property is set to the FriendlyName.</remarks>
    protected override async Task OnSubmitEditFormAsync()
    {
        DataObject.Name = DataObject.FriendlyName;
        await base.OnSubmitEditFormAsync();
    }
}
