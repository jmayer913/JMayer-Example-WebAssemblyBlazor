using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class manages comparing two StorageLocation objects.
/// </summary>
public class StorageLocationEqualityComparer : IEqualityComparer<StorageLocation>
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
    /// The property constructor.
    /// </summary>
    /// <param name="excludeCreatedOn">Excludes the CreatedOn property from the equals check.</param>
    /// <param name="exlucdeID">Excludes the ID property from the equals check.</param>
    /// <param name="excludeLastEditedOn">Excludes the LastEditedOn property from the equals check.</param>
    public StorageLocationEqualityComparer(bool excludeCreatedOn, bool exlucdeID, bool excludeLastEditedOn)
    {
        _excludeCreatedOn = excludeCreatedOn;
        _exlucdeID = exlucdeID;
        _excludeLastEditedOn = excludeLastEditedOn;
    }

    /// <inheritdoc/>
    public bool Equals(StorageLocation? x, StorageLocation? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return (_excludeCreatedOn || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && (_exlucdeID || x.Integer64ID == y.Integer64ID)
            && (_excludeLastEditedOn || x.LastEditedOn == y.LastEditedOn)
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
