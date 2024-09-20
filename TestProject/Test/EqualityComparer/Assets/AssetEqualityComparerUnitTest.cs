using JMayer.Example.WebAssemblyBlazor.Shared.Data;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace TestProject.Test.EqualityComparer.Assets;

/// <summary>
/// The class manages testing the asset equality comparer.
/// </summary>
public class AssetEqualityComparerUnitTest
{
    /// <summary>
    /// The constant for the category.
    /// </summary>
    private const string Category = "Category";

    /// <summary>
    /// The constant for the description.
    /// </summary>
    private const string Description = "Description";

    /// <summary>
    /// The constant for the ID.
    /// </summary>
    private const long ID = 1;

    /// <summary>
    /// The constant for the make.
    /// </summary>
    private const string Make = "Make";

    /// <summary>
    /// The constant for the manufacturer.
    /// </summary>
    private const string Manufacturer = "Manufacturer";

    /// <summary>
    /// The constant for the manufacturer.
    /// </summary>
    private const string ManufacturerNumber = "ManufacturerNumber";

    /// <summary>
    /// The constant for the model.
    /// </summary>
    private const string Model = "Model";

    /// <summary>
    /// The constant for the name.
    /// </summary>
    private const string Name = "Name";

    /// <summary>
    /// The constant for the parent ID.
    /// </summary>
    private const long ParentID = 1;

    /// <summary>
    /// The constant for the storage location ID.
    /// </summary>
    private const long StorageLocationID = 1;

    /// <summary>
    /// The method verifies equality failure when two nulls are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureBothNull() => Assert.False(new AssetEqualityComparer().Equals(null, null));

    /// <summary>
    /// The method verifies equality failure when the Category property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCategory()
    {
        Asset asset1 = new()
        {
            Category = Category,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the CreatedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCreatedOn()
    {
        Asset asset1 = new()
        {
            CreatedOn = DateTime.Now,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Description property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDescription()
    {
        Asset asset1 = new()
        {
            Description = Description,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureID()
    {
        Asset asset1 = new()
        {
            Integer64ID = ID,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the IsOnline property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureIsOnline()
    {
        Asset asset1 = new()
        {
            IsOnline = false,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the LastEditedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLastEditedOn()
    {
        Asset asset1 = new()
        {
            LastEditedOn = DateTime.Now,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Make property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureMake()
    {
        Asset asset1 = new()
        {
            Make = Make,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Manufacturer property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureManufacturer()
    {
        Asset asset1 = new()
        {
            Manufacturer = Manufacturer,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the ManufacturerNumber property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureManufacturerNumber()
    {
        Asset asset1 = new()
        {
            ManufacturerNumber = ManufacturerNumber,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Model property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureModel()
    {
        Asset asset1 = new()
        {
            Model = Model,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Name property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureName()
    {
        Asset asset1 = new()
        {
            Name = Name,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when an object and null are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureOneIsNull()
    {
        Asset asset = new()
        {
            Category = Category,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Make = Make,
            Manufacturer = Manufacturer,
            ManufacturerNumber = ManufacturerNumber,
            Model = Model,
            Name = Name,
            ParentID = ParentID,
            Priority = Priority.High,
            StorageLocationID = StorageLocationID,
            Type = AssetType.Group,
        };

        Assert.False(new AssetEqualityComparer().Equals(asset, null));
        Assert.False(new AssetEqualityComparer().Equals(null, asset));
    }

    /// <summary>
    /// The method verifies equality failure when the ParentID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureParentID()
    {
        Asset asset1 = new()
        {
            ParentID = ParentID,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Priority property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailurePriority()
    {
        Asset asset1 = new()
        {
            Priority = Priority.High,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the StorageLocationID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureStorageLocationID()
    {
        Asset asset1 = new()
        {
            StorageLocationID = StorageLocationID,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality failure when the Type property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureType()
    {
        Asset asset1 = new()
        {
            Type = AssetType.Group,
        };
        Asset asset2 = new();

        Assert.False(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared.
    /// </summary>
    [Fact]
    public void VerifySuccess()
    {
        Asset asset1 = new()
        {
            Category = Category,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Make = Make,
            Manufacturer = Manufacturer,
            ManufacturerNumber = ManufacturerNumber,
            Model = Model,
            Name = Name,
            ParentID = ParentID,
            Priority = Priority.High,
            StorageLocationID = StorageLocationID,
            Type = AssetType.Group,
        };
        Asset asset2 = new(asset1);

        Assert.True(new AssetEqualityComparer().Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeCreatedOn()
    {
        Asset asset1 = new()
        {
            Category = Category,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Make = Make,
            Manufacturer = Manufacturer,
            ManufacturerNumber = ManufacturerNumber,
            Model = Model,
            Name = Name,
            ParentID = ParentID,
            Priority = Priority.High,
            StorageLocationID = StorageLocationID,
            Type = AssetType.Group,
        };
        Asset asset2 = new(asset1)
        {
            CreatedOn = DateTime.MinValue,
        };

        Assert.True(new AssetEqualityComparer(true, false, false).Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the Integer64ID property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeID()
    {
        Asset asset1 = new()
        {
            Category = Category,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Make = Make,
            Manufacturer = Manufacturer,
            ManufacturerNumber = ManufacturerNumber,
            Model = Model,
            Name = Name,
            ParentID = ParentID,
            Priority = Priority.High,
            StorageLocationID = StorageLocationID,
            Type = AssetType.Group,
        };
        Asset asset2 = new(asset1)
        {
            Integer64ID = ID + 1,
        };

        Assert.True(new AssetEqualityComparer(false, true, false).Equals(asset1, asset2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeLastEditedOn()
    {
        Asset asset1 = new()
        {
            Category = Category,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Make = Make,
            Manufacturer = Manufacturer,
            ManufacturerNumber = ManufacturerNumber,
            Model = Model,
            Name = Name,
            ParentID = ParentID,
            Priority = Priority.High,
            StorageLocationID = StorageLocationID,
            Type = AssetType.Group,
        };
        Asset asset2 = new(asset1)
        {
            LastEditedOn = DateTime.MinValue,
        };

        Assert.True(new AssetEqualityComparer(false, false, true).Equals(asset1, asset2));
    }
}
