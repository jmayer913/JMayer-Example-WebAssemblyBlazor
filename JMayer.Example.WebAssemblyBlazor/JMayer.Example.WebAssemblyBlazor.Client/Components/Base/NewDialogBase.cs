﻿using JMayer.Data.Data;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Example.WebAssemblyBlazor.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Components.Base;

/// <summary>
/// The class manages user interaction for a new dialog.
/// </summary>
/// <typeparam name="T">Must be a UserEditableDataObject.</typeparam>
/// <typeparam name="U">Must be a IUserEditableDataLayer.</typeparam>
public class NewDialogBase<T, U> : ComponentBase
    where T : UserEditableDataObject, new()
    where U : IUserEditableDataLayer<T>
{
    /// <summary>
    /// The property gets/sets the data layer to be used by the dialog.
    /// </summary>
    [Inject]
    protected U DataLayer { get; set; } = default!;

    /// <summary>
    /// The property gets/sets the data object to create.
    /// </summary>
    protected T DataObject { get; set; } = new();

    /// <summary>
    /// The property gets/sets the dialog service used for managing MudDialogs.
    /// </summary>
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    /// <summary>
    /// The property gets/sets the edit context associated with the edit form.
    /// </summary>
    protected EditContext EditContext { get; set; } = default!;

    /// <summary>
    /// The property gets/sets a reference to the mud dialog.
    /// </summary>
    [CascadingParameter]
    protected IMudDialogInstance MudDialog { get; set; } = default!;

    /// <summary>
    /// The property gets/sets a reference to the server side validation.
    /// </summary>
    protected ServerSideValidation ServerSideValidation { get; set; } = default!;

    /// <summary>
    /// The method initializes the component.
    /// </summary>
    protected override void OnInitialized()
    {
        EditContext = new(DataObject);
        base.OnInitialized();
    }

    /// <summary>
    /// The method closes the dialog with a cancel result.
    /// </summary>
    protected virtual void OnCancelButtonClick() => MudDialog.Cancel();

    /// <summary>
    /// The method attempts to create a new data object on the server.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    protected virtual async Task OnSubmitEditFormAsync()
    {
        try
        {
            OperationResult operationResult = await DataLayer.CreateAsync(DataObject);

            if (operationResult.IsSuccessStatusCode)
            {
                MudDialog.Close();
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
                await DialogService.ShowErrorMessageAsync("Failed to create the object because of an error on the server.");
            }
        }
        catch
        {
            await DialogService.ShowErrorMessageAsync("Failed to communicate with the server.");
        }
    }
}
