using JMayer.Data.Data;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class represents an asset (equipment) that needs to be monitored and work orders can be preformed on it.
/// </summary>
public class Asset : UserEditableDataObject
{
    /// <summary>
    /// The property gets/sets the common category for the asset.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// The property gets/sets if the asset is online.
    /// </summary>
    public bool IsOnline { get; set; } = true;

    /// <summary>
    /// The property gets/sets the make of the asset.
    /// </summary>
    public string? Make { get; set; }

    /// <summary>
    /// The property gets/sets who makes the asset.
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// The property gets/sets the identifier the manufacturer uses for the asset.
    /// </summary>
    public string? ManufacturerNumber { get; set; }

    /// <summary>
    /// The property gets the parent path as if this asset is the parent.
    /// </summary>
    /// <remarks>
    /// This is only used by the backend when updating the parent path.
    /// </remarks>
    internal string MeAsParentPath
    {
        get => ParentID == null ? Name : $"{ParentPath}/{Name}";
    }

    /// <summary>
    /// The property gets/sets the model for the asset.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The property gets/sets id for the parent of this asset.
    /// </summary>
    public long? ParentID { get; set; }

    /// <summary>
    /// The property gets/sets the path of parents for this asset.
    /// </summary>
    public string? ParentPath { get; set; }

    /// <summary>
    /// The property gets/sets the how import the asset is to the system.
    /// </summary>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// The property gets/sets the storage location the asset is located in.
    /// </summary>
    /// <remarks>
    /// Only an equipment asset can be assigned a storage location.
    /// </remarks>
    public long StorageLocationID { get; set; }

    /// <summary>
    /// The property gets/sets what the asset represents.
    /// </summary>
    public AssetType Type { get; set; } = AssetType.Equipment;

    /// <inheritdoc/>
    public Asset() : base() { }

    /// <inheritdoc/>
    public Asset(Asset copy) : base(copy) { }

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is Asset asset)
        {
            Category = asset.Category;
            IsOnline = asset.IsOnline;
            Make = asset.Make;
            Manufacturer = asset.Manufacturer;
            ManufacturerNumber = asset.ManufacturerNumber;
            Model = asset.Model;
            ParentID = asset.ParentID;
            ParentPath = asset.ParentPath;
            Priority = asset.Priority;
            StorageLocationID = asset.StorageLocationID;
            Type = asset.Type;
        }
    }
}
