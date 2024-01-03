using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Components;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net;

namespace JMayer.Example.WebAssemblyBlazor.Client.Pages.Parts.Cards;

/// <summary>
/// The class manages user interactions with the OverviewCard.razor component.
/// </summary>
public class OverviewCardBase : ComponentBase
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
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
    /// The original part data before edits.
    /// </summary>
    private Part OriginalPart = new();

    /// <summary>
    /// The property gets/sets the part the overview will display information on.
    /// </summary>
    [Parameter]
    public Part Part { get; set; } = new();

    /// <summary>
    /// The property gets/sets a change event for the part.
    /// </summary>
    [Parameter]
    public EventCallback<Part> PartChanged { get; set; }

    /// <summary>
    /// The property gets/sets a reference to the server side validation.
    /// </summary>
    protected ServerSideValidation ServerSideValidation { get; set; } = null!;

    /// <summary>
    /// The method sets 
    /// </summary>
    protected override void OnParametersSet()
    {
        OriginalPart = new(Part);
        EditContext = new(Part);
        base.OnParametersSet();
    }

    /// <summary>
    /// The method resets the user's edits.
    /// </summary>
    /// <returns></returns>
    protected void OnCancelClick()
    {
        Part.MapProperties(OriginalPart);
        EditContext.MarkAsUnmodified();
    }

    /// <summary>
    /// The method attempts to update a part on the server.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected async Task OnSubmitFormAsync()
    {
        try
        {
            OperationResult operationResult = await DataLayer.UpdateAsync(Part);

            if (operationResult.IsSuccessStatusCode)
            {
                OriginalPart.MapProperties(Part);
                EditContext.MarkAsUnmodified();
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
            else if (operationResult.StatusCode == HttpStatusCode.Conflict)
            {
                await DialogService.ShowEditConflictMessageAsync();
            }
            else
            {
                await DialogService.ShowErrorMessageAsync("Failed to update the part because of an error on the server.");
            }
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }
    }
}
