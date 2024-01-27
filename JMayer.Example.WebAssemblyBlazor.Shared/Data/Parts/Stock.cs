using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

/// <summary>
/// The class represents stock for a part at a particular storage location.
/// </summary>
/// <remarks>
/// The OwnerId in the SubUserEditableDataObject will represent a part.
/// </remarks>
public class Stock : SubUserEditableDataObject
{
    /// <summary>
    /// The property gets/sets the amount of stock at the location for the part.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int Amount { get; set; }

    /// <summary>
    /// The property gets/sets the id for the storage location the part is stored at.
    /// </summary>
    [Required]
    public long StorageLocationId { get; set; }

    /// <inheritdoc/>
    public Stock() : base() { }

    /// <inheritdoc/>
    public Stock(Stock copy) : base(copy) { }

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is Stock stock)
        {
            Amount = stock.Amount;
            StorageLocationId = stock.StorageLocationId;
        }
    }
}
