using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

/// <summary>
/// The class manages comparing two Part objects.
/// </summary>
public class PartEqualityComparer : IEqualityComparer<Part>
{
    /// <inheritdoc/>
    public bool Equals(Part? x, Part? y)
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
               x.Make == y.Make &&
               x.Manufacturer == y.Manufacturer &&
               x.ManufacturerNumber == y.ManufacturerNumber &&
               x.Model == y.Model &&
               x.Name == y.Name &&
               x.Obsolete == y.Obsolete;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Part obj)
    {
        throw new NotImplementedException();
    }
}
