namespace JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

/// <summary>
/// The enumeration for the general types of assets.
/// </summary>
public enum AssetType
{
    /// <summary>
    /// An asset which represents an equipment that needs to be monitored. This
    /// type can be taken online/offline and work orders can be created for it.
    /// </summary>
    Equipment,
    
    /// <summary>
    /// An asset which represents an area of storage. This type can have storage
    /// locations;  where assets or parts are stored for inventory purposes. It cannot
    /// be taken online/offline and work orders cannot be created for it.
    /// </summary>
    Area,

    /// <summary>
    /// An asset which represents a common grouping of assets. Area or equipment assets
    /// would have this as a parent asset. It cannot be taken online/offline and work orders 
    /// cannot be created for it.
    /// </summary>
    Group,
}
