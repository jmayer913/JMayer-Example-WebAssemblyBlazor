using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class manages comparing two StorageLocation objects.
/// </summary>
public class StorageLocationEqualityComparer : IEqualityComparer<StorageLocation>
{
    /// <summary>
    /// Excludes auto assigned fields from the equals check.
    /// </summary>
    private readonly bool _excludeAutoAssignedFields;

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="excludeAutoAssignedFields">Are auto assigned fields excluded from the check?</param>
    public StorageLocationEqualityComparer(bool excludeAutoAssignedFields) => _excludeAutoAssignedFields = excludeAutoAssignedFields;

    /// <inheritdoc/>
    public bool Equals(StorageLocation? x, StorageLocation? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return (_excludeAutoAssignedFields || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && (_excludeAutoAssignedFields || x.Integer64ID == y.Integer64ID)
            && (_excludeAutoAssignedFields || x.LastEditedOn == y.LastEditedOn)
            && x.LocationA == y.LocationA
            && x.LocationB == y.LocationB
            && x.LocationC == y.LocationC
            && x.Name == y.Name
            && x.OwnerInteger64ID == y.OwnerInteger64ID;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] StorageLocation obj)
    {
        throw new NotImplementedException();
    }
}
