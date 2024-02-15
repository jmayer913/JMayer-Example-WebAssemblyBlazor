using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

/// <summary>
/// The class manages comparing two Stock objects.
/// </summary>
public class StockEqualityComparer : IEqualityComparer<Stock>
{
    /// <summary>
    /// Excludes auto assigned fields from the equals check.
    /// </summary>
    private readonly bool _excludeAutoAssignedFields;

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="excludeAutoAssignedFields">Are auto assigned fields excluded from the check?</param>
    public StockEqualityComparer(bool excludeAutoAssignedFields) => _excludeAutoAssignedFields = excludeAutoAssignedFields;

    /// <inheritdoc/>
    public bool Equals(Stock? x, Stock? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Amount == y.Amount
            && (_excludeAutoAssignedFields || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && (_excludeAutoAssignedFields || x.Integer64ID == y.Integer64ID)
            && (_excludeAutoAssignedFields || x.LastEditedOn == y.LastEditedOn)
            && x.Name == y.Name
            && x.OwnerInteger64ID == y.OwnerInteger64ID
            && x.StorageLocationId == y.StorageLocationId
            && x.StorageLocationName == y.StorageLocationName;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Stock obj)
    {
        throw new NotImplementedException();
    }
}
