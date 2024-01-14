using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net;

namespace JMayer.Example.WebAssemblyBlazor.Client.Components.Base;

/// <summary>
/// The class manages user interaction for an overview card.
/// </summary>
/// <typeparam name="T">Must be a UserEditableDataObject.</typeparam>
/// <typeparam name="U">Must be a IUserEditableDataLayer.</typeparam>
public class OverviewCardBase<T, U> : ComponentBase
    where T : UserEditableDataObject, new()
    where U : IUserEditableDataLayer<T>
{
    /// <summary>
    /// The property gets/sets the data layer to used by the page.
    /// </summary>
    [Inject]
    protected U DataLayer { get; set; }

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
    /// The original data object before edits.
    /// </summary>
    private readonly T OriginalDataObject = new();

    /// <summary>
    /// The property gets/sets the data object the overview will display information on.
    /// </summary>
    [Parameter]
    public T DataObject { get; set; } = new();

    /// <summary>
    /// The property gets/sets a change event for the data object.
    /// </summary>
    [Parameter]
    public EventCallback<T> DataObjectChanged { get; set; }

    /// <summary>
    /// The property gets/sets a reference to the server side validation.
    /// </summary>
    protected ServerSideValidation ServerSideValidation { get; set; } = null!;

    /// <summary>
    /// The method sets up the component after the parameters are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        OriginalDataObject.MapProperties(DataObject);
        EditContext = new(DataObject);
        base.OnParametersSet();
    }

    /// <summary>
    /// The method resets the user's edits.
    /// </summary>
    /// <returns></returns>
    protected virtual void OnCancelClick()
    {
        DataObject.MapProperties(OriginalDataObject);
        EditContext.MarkAsUnmodified();
    }

    /// <summary>
    /// The method attempts to update a data object on the server.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected virtual async Task OnSubmitFormAsync()
    {
        try
        {
            OperationResult operationResult = await DataLayer.UpdateAsync(DataObject);

            if (operationResult.IsSuccessStatusCode)
            {
                OriginalDataObject.MapProperties(DataObject);
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
