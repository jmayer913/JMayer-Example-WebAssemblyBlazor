using JMayer.Data.Data;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

/// <summary>
/// The class represents a part to be used for repairing an asset.
/// </summary>
public class Part : UserEditableDataObject
{
    /// <summary>
    /// The property gets/sets the common category for the part.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// The property gets/sets the make of the part.
    /// </summary>
    public string? Make { get; set; }

    /// <summary>
    /// The property gets/sets who makes the part.
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// The property gets/sets the identifier the manufacturer uses for the part.
    /// </summary>
    public string? ManufacturerNumber { get; set; }

    /// <summary>
    /// The property gets/sets the model for the part.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The property gets/sets is no longer procedured by the manfacturer.
    /// </summary>
    public bool Obsolete { get; set; }

    /// <inheritdoc/>
    public Part() : base() { }

    /// <inheritdoc/>
    public Part(Part copy) : base(copy) { }

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is Part partDataObject)
        {
            Category = partDataObject.Category;
            Make = partDataObject.Make;
            Manufacturer = partDataObject.Manufacturer;
            ManufacturerNumber = partDataObject.ManufacturerNumber;
            Model = partDataObject.Model;
            Obsolete = partDataObject.Obsolete;
        }
    }
}
