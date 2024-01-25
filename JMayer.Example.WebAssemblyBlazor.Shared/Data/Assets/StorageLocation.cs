using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class represents a storage location for an area asset.
/// </summary>
public class StorageLocation : UserEditableDataObject
{
    /// <summary>
    /// The property gets/sets the id for the area asset which owns this storage location.
    /// </summary>
    public long AssetId { get; set; }

    /// <summary>
    /// The property gets/sets the friendly name (locations concatenated).
    /// </summary>
    public string FriendlyName
    {
        get => $"{LocationA}{(string.IsNullOrWhiteSpace(LocationB) ? $" {LocationB}" : string.Empty)}{(string.IsNullOrWhiteSpace(LocationC) ? $" {LocationC}" : string.Empty)}";
    }

    /// <summary>
    /// The property gets/sets the name of the A location for the storage location.
    /// </summary>
    [Required]
    public string LocationA { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the name of the B location for the storage location.
    /// </summary>
    public string LocationB { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the name of the C location for the storage location.
    /// </summary>
    public string LocationC { get; set; } = string.Empty;

    /// <inheritdoc/>
    public StorageLocation() : base() { }

    /// <inheritdoc/>
    public StorageLocation(StorageLocation copy) : base(copy) { }

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is  StorageLocation storageLocation)
        {
            AssetId = storageLocation.AssetId;
            LocationA = storageLocation.LocationA;
            LocationB = storageLocation.LocationB;
            LocationC = storageLocation.LocationC;
        }
    }
}
