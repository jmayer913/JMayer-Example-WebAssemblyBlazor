using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class manages comparing two Asset objects.
/// </summary>
public class AssetEqualityComparer : IEqualityComparer<Asset>
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
    public AssetEqualityComparer(bool excludeCreatedOn, bool exlucdeID, bool excludeLastEditedOn)
    {
        _excludeCreatedOn = excludeCreatedOn;
        _exlucdeID = exlucdeID;
        _excludeLastEditedOn = excludeLastEditedOn;
    }

    /// <inheritdoc/>
    public bool Equals(Asset? x, Asset? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Category == y.Category
            && (_excludeCreatedOn || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && (_exlucdeID || x.Integer64ID == y.Integer64ID)
            && (_excludeLastEditedOn || x.LastEditedOn == y.LastEditedOn)
            && x.IsOnline == y.IsOnline 
            && x.Make == y.Make 
            && x.Manufacturer == y.Manufacturer 
            && x.ManufacturerNumber == y.ManufacturerNumber 
            && x.Model == y.Model 
            && x.Name == y.Name 
            && x.ParentID == y.ParentID 
            && x.Priority == y.Priority 
            && x.Type == y.Type;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Asset obj)
    {
        throw new NotImplementedException();
    }
}
