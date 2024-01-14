using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The class manages comparing two Asset objects.
/// </summary>
public class AssetEqualityComparer : IEqualityComparer<Asset>
{
    /// <inheritdoc/>
    public bool Equals(Asset? x, Asset? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Category == y.Category &&
               x.CreatedOn == y.CreatedOn &&
               x.Description == y.Description &&
               x.Integer64ID == y.Integer64ID &&
               x.LastEditedOn == y.LastEditedOn &&
               x.IsOnline == y.IsOnline &&
               x.Make == y.Make &&
               x.Manufacturer == y.Manufacturer &&
               x.ManufacturerNumber == y.ManufacturerNumber &&
               x.Model == y.Model &&
               x.Name == y.Name &&
               x.ParentID == y.ParentID &&
               x.Priority == y.Priority &&
               x.Type == y.Type;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Asset obj)
    {
        throw new NotImplementedException();
    }
}
