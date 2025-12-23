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
    /// The constant for the ATR category name.
    /// </summary>
    private const string AtrCategoryName = "ATR";

    /// <summary>
    /// The constant for the bag room category name.
    /// </summary>
    private const string BagRoomCategoryName = "Bag Room";

    /// <summary>
    /// The constant for the belt category name.
    /// </summary>
    private const string BeltCategoryName = "Belt";

    /// <summary>
    /// The constant for the conveyor category name.
    /// </summary>
    private const string ConveyorCategoryName = "Conveyor";

    /// <summary>
    /// The constant for the diverter category name.
    /// </summary>
    private const string DiverterCategoryName = "Diverter";

    /// <summary>
    /// The constant for the electrical contact category name.
    /// </summary>
    private const string ElectricalContactCategoryName = "Electrical Contact";

    /// <summary>
    /// The constant for the main part storage area asset name.
    /// </summary>
    public const string MainPartStorageAreaAssetName = "Main Part Storage";

    /// <summary>
    /// The constant for the conveyor length for the main sortation subsystem.
    /// </summary>
    private const int MainSortationConveyorLength = 15;

    /// <summary>
    /// The constant for the conveyor length for the makeup unit subsystem.
    /// </summary>
    private const int MakeupUnitConveyorLength = 6;

    /// <summary>
    /// The constant for the motor category name.
    /// </summary>
    private const string MotorCategoryName = "Motor";

    /// <summary>
    /// The property gets/sets the data layer used to interact with parts.
    /// </summary>
    public IPartDataLayer PartDataLayer { get; set; }

    /// <summary>
    /// The constant for the photoeye category name.
    /// </summary>
    private const string PhotoeyeCategoryName = "Photoeye";

    /// <summary>
    /// The property gets/sets the data layer used to interact with part stock.
    /// </summary>
    public IStockDataLayer StockDataLayer { get; set; }

    /// <summary>
    /// The constant for the storage category name.
    /// </summary>
    private const string StorageCategoryName = "Storage";

    /// <summary>
    /// The property gets/sets the data layer used to interact with asset storage locations.
    /// </summary>
    public IStorageLocationDataLayer StorageLocationDataLayer { get; set; }

    /// <summary>
    /// The constant for the subsystem category name.
    /// </summary>
    private const string SubSystemCategoryName = "SubSystem";

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
            Category = BagRoomCategoryName,
            Description = "The main bag room for the BHS.",
            Name = $"Main Bag Room",
            Type = AssetType.Group,
        }).Result;

        Asset asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = SubSystemCategoryName,
            Description = "The main sortation line for the bag room.",
            Name = "MS1",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = AtrCategoryName,
            Description = "The scanner array for reading bags on the main sortation line.",
            Name = "MS1-ATR",
            ParentID = asset.Integer64ID,
            Priority = Priority.High,
            Type = AssetType.Equipment,
        });

        for (int index = 1; index <= MainSortationConveyorLength; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = ConveyorCategoryName,
                Description = "The conveyor which makes up the main sortation line.",
                Name = $"MS1-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = SubSystemCategoryName,
            Description = "The one of the destination lines for the bag room.",
            Name = "MU1",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = DiverterCategoryName,
            Description = "The diverter which pushes bags onto the MU1 destination line.",
            Name = "MU1-DIV",
            ParentID = asset.Integer64ID,
            Priority = Priority.High,
            Type = AssetType.Equipment,
        });

        for (int index = 1; index <= MakeupUnitConveyorLength; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = ConveyorCategoryName,
                Description = "The conveyor which makes up the MU1 destination line.",
                Name = $"MU1-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = SubSystemCategoryName,
            Description = "The one of the destination lines for the bag room.",
            Name = "MU2",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = DiverterCategoryName,
            Description = "The diverter which pushes bags onto the MU2 destination line.",
            Name = "MU2-DIV",
            ParentID = asset.Integer64ID,
            Priority = Priority.High,
            Type = AssetType.Equipment,
        });

        for (int index = 1; index <= MakeupUnitConveyorLength; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = ConveyorCategoryName,
                Description = "The conveyor which makes up the MU2 destination line.",
                Name = $"MU2-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = AssetDataLayer.CreateAsync(new Asset()
        {
            Category = SubSystemCategoryName,
            Description = "The runout line for the bag room.",
            Name = "MU3",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        for (int index = 1; index <= MakeupUnitConveyorLength; index++)
        {
            _ = AssetDataLayer.CreateAsync(new Asset()
            {
                Category = ConveyorCategoryName,
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
            Category = BeltCategoryName,
            Name = "Power Turn Belt",
        });
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = ElectricalContactCategoryName,
            Name = "Motor Contactor",
        });
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = MotorCategoryName,
            Name = "Motor 1HP Drive",
        });
        _ = PartDataLayer.CreateAsync(new Part()
        {
            Category = PhotoeyeCategoryName,
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
                    OwnerInteger64ID = parts[index].Integer64ID,
                    StorageLocationID = storageLocations[index].Integer64ID,
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
            Category = StorageCategoryName,
            Description = "The main part storage for the BHS.",
            Name = MainPartStorageAreaAssetName,
            Type = AssetType.Area,
        }).Result;

        StorageLocation storageLocation = new()
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
