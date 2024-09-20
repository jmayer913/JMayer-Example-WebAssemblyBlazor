using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace TestProject.Test.EqualityComparer.Parts;

/// <summary>
/// The class manages testing the stock equality comparer.
/// </summary>
public class StockEqualityComparerUnitTest
{
    /// <summary>
    /// The constant for the amount.
    /// </summary>
    private const decimal Amount = 100;

    /// <summary>
    /// The constant for the description.
    /// </summary>
    private const string Description = "Description";

    /// <summary>
    /// The constant for the ID.
    /// </summary>
    private const long ID = 1;

    /// <summary>
    /// The constant for the name.
    /// </summary>
    private const string Name = "Name";

    /// <summary>
    /// The constant for the owner ID.
    /// </summary>
    private const long OwnerID = 1;

    /// <summary>
    /// The constant for the storage location ID.
    /// </summary>
    private const long StorageLocationID = 1;

    /// <summary>
    /// The constant for the storage location name.
    /// </summary>
    private const string StorageLocationName = "Storage Location Name";

    /// <summary>
    /// The method verifies equality failure when the Amount property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureAmount()
    {
        Stock stock1 = new()
        {
            Amount = Amount,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the CreatedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCreatedOn()
    {
        Stock stock1 = new()
        {
            CreatedOn = DateTime.Now,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the Description property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDescription()
    {
        Stock stock1 = new()
        {
            Description = Description,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureID()
    {
        Stock stock1 = new()
        {
            Integer64ID = ID,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the LastEditedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLastEditedOn()
    {
        Stock stock1 = new()
        {
            LastEditedOn = DateTime.Now,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the Name property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureName()
    {
        Stock stock1 = new()
        {
            Name = Name,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when an object and null are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureOneIsNull()
    {
        Stock stock = new()
        {
            Amount = Amount,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OwnerInteger64ID = OwnerID,
            StorageLocationID = StorageLocationID,
            StorageLocationName = StorageLocationName,
        };

        Assert.False(new StockEqualityComparer().Equals(stock, null));
        Assert.False(new StockEqualityComparer().Equals(null, stock));
    }

    /// <summary>
    /// The method verifies equality failure when the OwnerID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureOwnerID()
    {
        Stock stock1 = new()
        {
            OwnerInteger64ID = OwnerID,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the StorageLocationID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureStorageLocationID()
    {
        Stock stock1 = new()
        {
            StorageLocationID = StorageLocationID,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality failure when the StorageLocationName property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureStorageLocationName()
    {
        Stock stock1 = new()
        {
            StorageLocationName = StorageLocationName,
        };
        Stock stock2 = new();

        Assert.False(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared.
    /// </summary>
    [Fact]
    public void VerifySuccess()
    {
        Stock stock1 = new()
        {
            Amount = Amount,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OwnerInteger64ID = OwnerID,
            StorageLocationID = StorageLocationID,
            StorageLocationName = StorageLocationName,
        };
        Stock stock2 = new(stock1);

        Assert.True(new StockEqualityComparer().Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeCreatedOn()
    {
        Stock stock1 = new()
        {
            Amount = Amount,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OwnerInteger64ID = OwnerID,
            StorageLocationID = StorageLocationID,
            StorageLocationName = StorageLocationName,
        };
        Stock stock2 = new(stock1)
        {
            CreatedOn = DateTime.MinValue,
        };

        Assert.True(new StockEqualityComparer(true, false, false).Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the Integer64ID property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeID()
    {
        Stock stock1 = new()
        {
            Amount = Amount,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OwnerInteger64ID = OwnerID,
            StorageLocationID = StorageLocationID,
            StorageLocationName = StorageLocationName,
        };
        Stock stock2 = new(stock1)
        {
            Integer64ID = ID + 1,
        };

        Assert.True(new StockEqualityComparer(false, true, false).Equals(stock1, stock2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeLastEditedOn()
    {
        Stock stock1 = new()
        {
            Amount = Amount,
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OwnerInteger64ID = OwnerID,
            StorageLocationID = StorageLocationID,
            StorageLocationName = StorageLocationName,
        };
        Stock stock2 = new(stock1)
        {
            LastEditedOn = DateTime.MinValue,
        };

        Assert.True(new StockEqualityComparer(false, false, true).Equals(stock1, stock2));
    }
}
