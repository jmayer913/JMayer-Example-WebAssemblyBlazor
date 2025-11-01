using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

#warning I wonder if I should make the locations nullable since no value can be set.
#warning I should use Name instead of FriendlyName; maybe I can override Name only have it be get or hide the set with the access level.

/// <summary>
/// The class represents a storage location for an area asset.
/// </summary>
/// <remarks>
/// The OwnerId in the SubDataObject will represent an asset.
/// </remarks>
public class StorageLocation : SubDataObject
{
    /// <summary>
    /// The property gets/sets the friendly name (locations concatenated).
    /// </summary>
    public string FriendlyName => $"{LocationA}{(string.IsNullOrWhiteSpace(LocationB) is false ? $" {LocationB}" : string.Empty)}{(string.IsNullOrWhiteSpace(LocationC) is false ? $" {LocationC}" : string.Empty)}";

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

        if (dataObject is StorageLocation storageLocation)
        {
            LocationA = storageLocation.LocationA;
            LocationB = storageLocation.LocationB;
            LocationC = storageLocation.LocationC;
        }
    }
}
