using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;

/// <summary>
/// The class is used to build BHS assets for an example application.
/// </summary>
public class AssetBHSExampleBuilder
{
    /// <summary>
    /// The property gets/sets the data layer the builder will interact with.
    /// </summary>
    public IAssetDataLayer DataLayer { get; set; } = null!;

    /// <summary>
    /// The method builds the example.
    /// </summary>
    public void Build()
    {
        _ = DataLayer.CreateAsync(new Asset()
        {
            Description = "The main part storage for the BHS.",
            Name = "Main Part Storage",
            Type = AssetType.Area,
        });

        Asset bagRoomAsset = DataLayer.CreateAsync(new Asset()
        {
            Category = "Bag Room",
            Description = "The main bag room for the BHS.",
            Name = "Main Bag Room",
            Type = AssetType.Group,
        }).Result;

        Asset asset = DataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The main sortation line for the bag room.",
            Name = "MS1",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = DataLayer.CreateAsync(new Asset()
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
            _ = DataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the main sortation line.",
                Name = $"MS1-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = DataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The one of the destination lines for the bag room.",
            Name = "MU1",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = DataLayer.CreateAsync(new Asset()
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
            _ = DataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the MU1 destination line.",
                Name = $"MU1-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = DataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The one of the destination lines for the bag room.",
            Name = "MU2",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        _ = DataLayer.CreateAsync(new Asset()
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
            _ = DataLayer.CreateAsync(new Asset()
            {
                Category = "Conveyor",
                Description = "The conveyor which makes up the MU2 destination line.",
                Name = $"MU2-{index:00}",
                ParentID = asset.Integer64ID,
                Priority = Priority.High,
                Type = AssetType.Equipment,
            });
        }

        asset = DataLayer.CreateAsync(new Asset()
        {
            Category = "SubSystem",
            Description = "The runout line for the bag room.",
            Name = "MU3",
            ParentID = bagRoomAsset.Integer64ID,
            Type = AssetType.Group,
        }).Result;

        for (int index = 1; index <= 6; index++)
        {
            _ = DataLayer.CreateAsync(new Asset()
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
}
