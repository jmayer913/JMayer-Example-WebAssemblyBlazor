using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;

/// <summary>
/// The class manages CRUD interactions with the database for a part.
/// </summary>
public class PartDataLayer : StandardCRUDDataLayer<Part>, IPartDataLayer
{
    /// <summary>
    /// The default constructor.
    /// </summary>
    public PartDataLayer()
    {
        IsOldDataObjectDetectionEnabled = true;
        IsUniqueNameRequired = true;
    }
}
