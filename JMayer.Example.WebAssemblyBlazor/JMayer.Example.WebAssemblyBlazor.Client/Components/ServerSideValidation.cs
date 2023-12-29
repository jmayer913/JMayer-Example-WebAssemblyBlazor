using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace JMayer.Example.WebAssemblyBlazor.Client.Components;

/// <summary>
/// The class manages displaying server-side validation errors on the edit form.
/// </summary>
public class ServerSideValidation : ComponentBase
{
    /// <summary>
    /// Stores the validation messages.
    /// </summary>
    private ValidationMessageStore? _store;

    /// <summary>
    /// The property gets/sets the edit context associated with the edit form.
    /// </summary>
    [CascadingParameter]
    public EditContext EditContext { get; set; } = null!;

    /// <summary>
    /// The method displays the errors on the edit form.
    /// </summary>
    /// <param name="errors">The errors to display.</param>
    public void DisplayErrors(Dictionary<string, List<string>> errors)
    {
        if (_store != null)
        {
            foreach (var error in errors)
            {
                _store.Add(EditContext.Field(error.Key), error.Value);
            }

            EditContext.NotifyValidationStateChanged();
        }
    }

    /// <summary>
    /// The method sets up the store and registers the necessary events on the edit context.
    /// </summary>
    protected override void OnInitialized()
    {
        _store = new ValidationMessageStore(EditContext);
        EditContext.OnValidationRequested += (s, e) => _store.Clear();
        EditContext.OnFieldChanged += (s, e) => _store.Clear(e.FieldIdentifier);
        base.OnInitialized();
    }
}
