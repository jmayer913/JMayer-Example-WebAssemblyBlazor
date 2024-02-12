using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class manages comparing two Asset objects.
/// </summary>
public class AssetEqualityComparer : IEqualityComparer<Asset>
{
    /// <summary>
    /// Excludes auto assigned fields from the equals check.
    /// </summary>
    private readonly bool _excludeAutoAssignedFields;

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="excludeAutoAssignedFields">Are auto assigned fields excluded from the check?</param>
    public AssetEqualityComparer(bool excludeAutoAssignedFields) => _excludeAutoAssignedFields = excludeAutoAssignedFields;

    /// <inheritdoc/>
    public bool Equals(Asset? x, Asset? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Category == y.Category
            && (_excludeAutoAssignedFields || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && (_excludeAutoAssignedFields || x.Integer64ID == y.Integer64ID)
            && (_excludeAutoAssignedFields || x.LastEditedOn == y.LastEditedOn)
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
