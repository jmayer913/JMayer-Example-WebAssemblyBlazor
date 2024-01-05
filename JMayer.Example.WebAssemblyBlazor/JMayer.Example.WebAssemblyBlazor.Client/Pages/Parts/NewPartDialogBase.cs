using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Components;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts;

/// <summary>
/// The class manages user interactions with the NewPartDialog.razor dialog.
/// </summary>
public class NewPartDialogBase : ComponentBase
{
    /// <summary>
    /// The property gets/sets the categories for the parts.
    /// </summary>
    protected List<string> Categories { get; set; } = [];

    /// <summary>
    /// The property gets/sets the data layer to be used by the dialog.
    /// </summary>
    [Inject]
    protected IPartDataLayer DataLayer { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the dialog service used for managing MudDialogs.
    /// </summary>
    [Inject]
    protected IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// The property gets/sets the edit context associated with the edit form.
    /// </summary>
    protected EditContext EditContext { get; set; } = null!;

    /// <summary>
    /// The property gets/sets a reference to the mud dialog.
    /// </summary>
    [CascadingParameter]
    protected MudDialogInstance? MudDialog { get; set; }

    /// <summary>
    /// The property gets/sets the part to create.
    /// </summary>
    protected Part Part { get; set; } = new();

    /// <summary>
    /// The property gets/sets a reference to the server side validation.
    /// </summary>
    protected ServerSideValidation ServerSideValidation { get; set; } = null!;

    /// <summary>
    /// The method initializes the component.
    /// </summary>
    protected override void OnInitialized()
    {
        EditContext = new(Part);
        base.OnInitialized();
    }

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
    /// The method closes the dialog with a cancel result.
    /// </summary>
    protected void OnCancelButtonClick() => MudDialog?.Cancel();

    /// <summary>
    /// The method returns the list based on what the user has typed in.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>A list of acceptable categories.</returns>
    protected async Task<IEnumerable<string>> OnCategoryAutoCompleteSearch(string value)
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

    /// <summary>
    /// The method attempts to create a new part on the server.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected async Task OnSubmitFormAsync()
    {
        try
        {
            OperationResult operationResult = await DataLayer.CreateAsync(Part);

            if (operationResult.IsSuccessStatusCode)
            {
                MudDialog?.Close();
            }
            else if (operationResult.ServerSideValidationResult?.Errors.Count > 0)
            {
                Dictionary<string, List<string>> errors = [];

                foreach (ServerSideValidationError error in operationResult.ServerSideValidationResult.Errors)
                {
                    errors.Add(error.PropertyName, [error.ErrorMessage]);
                }

                ServerSideValidation.DisplayErrors(errors);
            }
            else
            {
                await DialogService.ShowErrorMessageAsync("Failed to create the part because of an error on the server.");
            }
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }
    }
}
