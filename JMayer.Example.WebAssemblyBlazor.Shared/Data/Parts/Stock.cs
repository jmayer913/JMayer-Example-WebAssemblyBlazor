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
    public decimal Amount { get; set; }

    /// <summary>
    /// The property gets/sets the id for the storage location the part is stored at.
    /// </summary>
    [Required]
    public long StorageLocationID { get; set; }

    /// <summary>
    /// The property gets/sets the name for the storage location the part is stored at.
    /// </summary>
    /// <remarks>
    /// This is only used to display the storage location name in the card grid.
    /// </remarks>
    public string StorageLocationName { get; set; } = string.Empty;

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
            StorageLocationID = stock.StorageLocationID;
            StorageLocationName = stock.StorageLocationName;
        }
    }
}
