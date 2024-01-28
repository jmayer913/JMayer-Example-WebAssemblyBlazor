using JMayer.Example.WebAssemblyBlazor.Client.Components.Base;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using Microsoft.AspNetCore.Components;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Assets.Dialogs;

/// <summary>
/// The class manages user interactions with the AddEditStorageLocationDialog.razor dialog.
/// </summary>
public class AddEditStorageLocationDialogBase : AddEditCardDialogBase<StorageLocation, IStorageLocationDataLayer>
{
    /// <inheritdoc/>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        //The Name property is a required field but the
        //StorageLocation data object doesn't use it so
        //it needs to be set to pass validation.
        DataObject.Name = "A Name";
        return base.SetParametersAsync(parameters);
    }

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
