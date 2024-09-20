using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

/// <summary>
/// The class manages comparing two Stock objects.
/// </summary>
public class StockEqualityComparer : IEqualityComparer<Stock>
{
    /// <summary>
    /// Excludes the CreatedOn property from the equals check.
    /// </summary>
    private readonly bool _excludeCreatedOn;

    /// <summary>
    /// Excludes the ID property from the equals check.
    /// </summary>
    private readonly bool _exlucdeID;

    /// <summary>
    /// Excludes the LastEditedOn property from the equals check.
    /// </summary>
    private readonly bool _excludeLastEditedOn;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public StockEqualityComparer() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="excludeCreatedOn">Excludes the CreatedOn property from the equals check.</param>
    /// <param name="exlucdeID">Excludes the ID property from the equals check.</param>
    /// <param name="excludeLastEditedOn">Excludes the LastEditedOn property from the equals check.</param>
    public StockEqualityComparer(bool excludeCreatedOn, bool exlucdeID, bool excludeLastEditedOn)
    {
        _excludeCreatedOn = excludeCreatedOn;
        _exlucdeID = exlucdeID;
        _excludeLastEditedOn = excludeLastEditedOn;
    }

    /// <inheritdoc/>
    public bool Equals(Stock? x, Stock? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Amount == y.Amount
            && (_excludeCreatedOn || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && (_exlucdeID || x.Integer64ID == y.Integer64ID)
            && (_excludeLastEditedOn || x.LastEditedOn == y.LastEditedOn)
            && x.Name == y.Name
            && x.OwnerInteger64ID == y.OwnerInteger64ID
            && x.StorageLocationID == y.StorageLocationID
            && x.StorageLocationName == y.StorageLocationName;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Stock obj)
    {
        throw new NotImplementedException();
    }
}
