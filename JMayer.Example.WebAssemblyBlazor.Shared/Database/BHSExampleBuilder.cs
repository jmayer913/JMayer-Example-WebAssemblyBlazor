using JMayer.Example.WebAssemblyBlazor.Shared.Data;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database;

/// <summary>
/// The class is used to generate example data for a BHS.
/// </summary>
public class BHSExampleBuilder
{
    /// <summary>
    /// The property gets/sets the data layer used to interact with assets.
    /// </summary>
    public IAssetDataLayer AssetDataLayer { get; set; }

    /// <summary>
    /// The property gets/sets the data layer used to interact with parts.
    /// </summary>
    public IPartDataLayer PartDataLayer { get; set; }

    /// <summary>
    /// The property gets/sets the data layer used to interact with part stock.
    /// </summary>
    public IStockDataLayer StockDataLayer { get; set; }

    /// <summary>
    /// The property gets/sets the data layer used to interact with asset storage locations.
    /// </summary>
    public IStorageLocationDataLayer StorageLocationDataLayer { get; set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public BHSExampleBuilder()
    {
        AssetDataLayer = new AssetDataLayer();
        PartDataLayer = new PartDataLayer();
        StorageLocationDataLayer = new StorageLocationDataLayer(AssetDataLayer);
        StockDataLayer = new StockDataLayer(PartDataLayer, StorageLocationDataLayer);
    }

    /// <summary>
    /// The method builds the BHS example data.
    /// </summary>
    public void Build()
    {
        BuildAssets();
        BuildStorageLocations();
        BuildParts();
        BuildStock();
    }

    /// <summary>
    /// The method builds the BHS assets.
    /// </summary>
    private void BuildAssets()
    {
        Asset bagRoomAsset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "Bag Room",
            Description = "The main bag room for the BHS.",
            Name = "Main Bag Room",
            Type = AssetType.Group,
        }).Result;

        Asset asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The main sortation line for the bag room.",
            Name = "MS1",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "ATR",
            Description = "The scanner array for reading bags on the main sortation line.",
            Name = "MS1-ATR",
            ParentID = asset.Integer64ID,
            Priority = Priority.High,
            Type = AssetType.Equipment,
        });

        for (int index = 1; index <= 15; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the main sortation line.",
                Name = $"MS1-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The one of the destination lines for the bag room.",
            Name = "MU1",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "Diverter",
            Description = "The diverter which pushes bags onto the MU1 destination line.",
            Name = "MU1-DIV",
            ParentID = asset.Integer64ID,
            Priority = Priority.High,
            Type = AssetType.Equipment,
        });

        for (int index = 1; index <= 6; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the MU1 destination line.",
                Name = $"MU1-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The one of the destination lines for the bag room.",
            Name = "MU2",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "Diverter",
            Description = "The diverter which pushes bags onto the MU2 destination line.",
            Name = "MU2-DIV",
            ParentID = asset.Integer64ID,
            Priority = Priority.High,
            Type = AssetType.Equipment,
        });

        for (int index = 1; index <= 6; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the MU2 destination line.",
                Name = $"MU2-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The runout line for the bag room.",
            Name = "MU3",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        for (int index = 1; index <= 6; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the MU3 destination line.",
                Name = $"MU3-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }
    }

    /// <summary>
    /// The method builds the parts needed for the BHS.
    /// </summary>
    private void BuildParts()
    {
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = "Belt",
            Name = "Power Turn Belt",
        });
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = "Contact",
            Name = "Motor Contactor",
        });
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = "Motor",
            Name = "Motor 1HP Drive",
        });
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = "Photoeye",
            Name = "Photoeye, Polarized Retro Reflective",
        });
    }

    /// <summary>
    /// The method builds the stock of the BHS.
    /// </summary>
    private void BuildStock()
    {
        List<Part> parts = PartDataLayer.GetAllAsync().Result;
        List<StorageLocation> storageLocations = StorageLocationDataLayer.GetAllAsync().Result;

        if (parts.Count == storageLocations.Count)
        {
            for (int index = 0; index < parts.Count; index++)
            {
                _ = StockDataLayer.CreateAsync(new Stock()
                {
                    Amount = 5 * (index + 1),
                    Name = "A Name",
                    OwnerInteger64ID = parts[index].Integer64ID,
                    StorageLocationId = storageLocations[index].Integer64ID,
                    StorageLocationName = storageLocations[index].FriendlyName,
                });
            }
        }
    }

    /// <summary>
    /// The method builds the storage locations.
    /// </summary>
    private void BuildStorageLocations()
    {
        Asset asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Description = "The main part storage for the BHS.",
            Name = "Main Part Storage",
            Type = AssetType.Area,
        }).Result;

        StorageLocation storageLocation = new StorageLocation()
        {
            LocationA = "A1",
            LocationB = "R1",
            LocationC = "S1",
            OwnerInteger64ID = asset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        _ = StorageLocationDataLayer.CreateAsync(storageLocation);

        storageLocation = new StorageLocation()
        {
            LocationA = "A1",
            LocationB = "R1",
            LocationC = "S2",
            OwnerInteger64ID = asset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        _ = StorageLocationDataLayer.CreateAsync(storageLocation);

        storageLocation = new StorageLocation()
        {
            LocationA = "A1",
            LocationB = "R2",
            LocationC = "S1",
            OwnerInteger64ID = asset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        _ = StorageLocationDataLayer.CreateAsync(storageLocation);

        storageLocation = new StorageLocation()
        {
            LocationA = "A1",
            LocationB = "R2",
            LocationC = "S2",
            OwnerInteger64ID = asset.Integer64ID,
        };
        storageLocation.Name = storageLocation.FriendlyName;
        _ = StorageLocationDataLayer.CreateAsync(storageLocation);
    }
}
