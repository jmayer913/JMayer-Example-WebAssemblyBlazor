using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace TestProject.Test.EqualityComparer.Assets;

/// <summary>
/// The class manages testing the storage location equality comparer.
/// </summary>
public class StorageLocationEqualityComparerUnitTest
{
    /// <summary>
    /// The constant for the description.
    /// </summary>
    private const string Description = "Description";

    /// <summary>
    /// The constant for the ID.
    /// </summary>
    private const long ID = 1;

    /// <summary>
    /// The constant for the location A.
    /// </summary>
    private const string LocationA = "LocationA";

    /// <summary>
    /// The constant for the location B.
    /// </summary>
    private const string LocationB = "LocationB";

    /// <summary>
    /// The constant for the location C.
    /// </summary>
    private const string LocationC = "LocationC";

    /// <summary>
    /// The constant for the name.
    /// </summary>
    private const string Name = "Name";

    /// <summary>
    /// The constant for the owner ID.
    /// </summary>
    private const long OwnerID = 1;

    /// <summary>
    /// The method verifies equality failure when two nulls are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureBothNull() => Assert.False(new StorageLocationEqualityComparer().Equals(null, null));

    /// <summary>
    /// The method verifies equality failure when the Description property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDescription()
    {
        StorageLocation storageLocation1 = new()
        {
            Description = Description,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the CreatedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCreatedOn()
    {
        StorageLocation storageLocation1 = new()
        {
            CreatedOn = DateTime.Now,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the Integer64ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureID()
    {
        StorageLocation storageLocation1 = new()
        {
            Integer64ID = ID,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the LastEditedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLastEditedOn()
    {
        StorageLocation storageLocation1 = new()
        {
            LastEditedOn = DateTime.Now,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the LocationA property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLocationA()
    {
        StorageLocation storageLocation1 = new()
        {
            LocationA = LocationA,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the LocationB property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLocationB()
    {
        StorageLocation storageLocation1 = new()
        {
            LocationB = LocationB,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the LocationC property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLocationC()
    {
        StorageLocation storageLocation1 = new()
        {
            LocationC = LocationC,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when the Name property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureName()
    {
        StorageLocation storageLocation1 = new()
        {
            Name = Name,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality failure when an object and null are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureOneIsNull()
    {
        StorageLocation storageLocation = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            LocationA = LocationA,
            LocationB = LocationB,
            LocationC = LocationC,
            Name = Name,
            OwnerInteger64ID = OwnerID,
        };

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation, null));
        Assert.False(new StorageLocationEqualityComparer().Equals(null, storageLocation));
    }

    /// <summary>
    /// The method verifies equality failure when the OwnerInteger64ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureOwnerID()
    {
        StorageLocation storageLocation1 = new()
        {
            OwnerInteger64ID = OwnerID,
        };
        StorageLocation storageLocation2 = new();

        Assert.False(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared.
    /// </summary>
    [Fact]
    public void VerifySuccess()
    {
        StorageLocation storageLocation1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            LocationA = LocationA,
            LocationB = LocationB,
            LocationC = LocationC,
            Name = Name,
            OwnerInteger64ID = OwnerID,
        };
        StorageLocation storageLocation2 = new(storageLocation1);

        Assert.True(new StorageLocationEqualityComparer().Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeCreatedOn()
    {
        StorageLocation storageLocation1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            LocationA = LocationA,
            LocationB = LocationB,
            LocationC = LocationC,
            Name = Name,
            OwnerInteger64ID = OwnerID,
        };
        StorageLocation storageLocation2 = new(storageLocation1)
        {
            CreatedOn = DateTime.MinValue,
        };

        Assert.True(new StorageLocationEqualityComparer(true, false, false).Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the Integer64ID property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeID()
    {
        StorageLocation storageLocation1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            LocationA = LocationA,
            LocationB = LocationB,
            LocationC = LocationC,
            Name = Name,
            OwnerInteger64ID = OwnerID,
        };
        StorageLocation storageLocation2 = new(storageLocation1)
        {
            Integer64ID = ID + 1,
        };

        Assert.True(new StorageLocationEqualityComparer(false, true, false).Equals(storageLocation1, storageLocation2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeLastEditedOn()
    {
        StorageLocation storageLocation1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            LocationA = LocationA,
            LocationB = LocationB,
            LocationC = LocationC,
            Name = Name,
            OwnerInteger64ID = OwnerID,
        };
        StorageLocation storageLocation2 = new(storageLocation1)
        {
            LastEditedOn = DateTime.MinValue,
        };

        Assert.True(new StorageLocationEqualityComparer(false, false, true).Equals(storageLocation1, storageLocation2));
    }
}
