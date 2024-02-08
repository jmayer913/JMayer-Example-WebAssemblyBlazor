using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Extensions;

/// <summary>
/// The static class contains extension methods for the IDialogService interface.
/// </summary>
public static class IDialogServiceExtension
{
    /// <summary>
    /// The method displays a confirm action message to the user.
    /// </summary>
    /// <param name="dialogService">The dialog service used to display the confirm action message.</param>
    /// <param name="message">The message to display to the user.</param>
    /// <returns>True means the user confirm the action; false or null means the user didn't.</returns>
    public static async Task<bool?> ShowConfirmActionMessageAsync(this IDialogService dialogService, string message = "Do you want to confirm this action?")
    {
        return await dialogService.ShowMessageBox("Warning", message, cancelText: "Cancel");
    }

    /// <summary>
    /// The method displays an edit conflict message to the user.
    /// </summary>
    /// <param name="dialogService">The dialog service used to display the error message.</param>
    /// <param name="message">The message to display to the user.</param>
    /// <returns>A Task object for the async.</returns>
    public static async Task ShowEditConflictMessageAsync(this IDialogService dialogService, string message = "The submitted data was detected to be out of date; please refresh and try again.")
    {
        _ = await dialogService.ShowMessageBox("Warning", message, options: new DialogOptions() { CloseButton = false });
    }

    /// <summary>
    /// The method displays an error message to the user.
    /// </summary>
    /// <param name="dialogService">The dialog service used to display the error message.</param>
    /// <param name="message">The message to display to the user.</param>
    /// <returns>A Task object for the async.</returns>
    public static async Task ShowErrorMessageAsync(this IDialogService dialogService, string message)
    {
        _ = await dialogService.ShowMessageBox("Error", message, options: new DialogOptions() { CloseButton = false });
    }
}
