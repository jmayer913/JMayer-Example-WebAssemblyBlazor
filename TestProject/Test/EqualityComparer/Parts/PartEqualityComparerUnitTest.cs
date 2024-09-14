using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;

namespace TestProject.Test.EqualityComparer.Parts;

/// <summary>
/// The class manages testing the part equality comparer.
/// </summary>
public class PartEqualityComparerUnitTest
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
    /// The method verifies equality failure when two nulls are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureBothNull() => Assert.False(new PartEqualityComparer().Equals(null, null));

    /// <summary>
    /// The method verifies equality failure when the Category property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCategory()
    {
        Part part1 = new()
        {
            Category = Category,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the CreatedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCreatedOn()
    {
        Part part1 = new()
        {
            CreatedOn = DateTime.Now,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the Description property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDescription()
    {
        Part part1 = new()
        {
            Description = Description,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureID()
    {
        Part part1 = new()
        {
            Integer64ID = ID,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the LastEditedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLastEditedOn()
    {
        Part part1 = new()
        {
            LastEditedOn = DateTime.Now,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the Make property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureMake()
    {
        Part part1 = new()
        {
            Make = Make,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the Manufacturer property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureManufacturer()
    {
        Part part1 = new()
        {
            Manufacturer = Manufacturer,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the ManufacturerNumber property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureManufacturerNumber()
    {
        Part part1 = new()
        {
            ManufacturerNumber = ManufacturerNumber,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the Model property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureModel()
    {
        Part part1 = new()
        {
            Model = Model,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the Name property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureName()
    {
        Part part1 = new()
        {
            Name = Name,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when the Obsolete property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureObsolete()
    {
        Part part1 = new()
        {
            Obsolete = true,
        };
        Part part2 = new();

        Assert.False(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality failure when an object and null are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureOneIsNull()
    {
        Part part = new()
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
            Obsolete = true,
        };

        Assert.False(new PartEqualityComparer().Equals(part, null));
        Assert.False(new PartEqualityComparer().Equals(null, part));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared.
    /// </summary>
    [Fact]
    public void VerifySuccess()
    {
        Part part1 = new()
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
            Obsolete = true,
        };
        Part part2 = new(part1);

        Assert.True(new PartEqualityComparer().Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeCreatedOn()
    {
        Part part1 = new()
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
            Obsolete = true,
        };
        Part part2 = new(part1)
        {
            CreatedOn = DateTime.MinValue,
        };

        Assert.True(new PartEqualityComparer(true, false, false).Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the Integer64ID property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeID()
    {
        Part part1 = new()
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
            Obsolete = true,
        };
        Part part2 = new(part1)
        {
            Integer64ID = ID + 1,
        };

        Assert.True(new PartEqualityComparer(false, true, false).Equals(part1, part2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeLastEditedOn()
    {
        Part part1 = new()
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
            Obsolete = true,
        };
        Part part2 = new(part1)
        {
            LastEditedOn = DateTime.MinValue,
        };

        Assert.True(new PartEqualityComparer(false, false, true).Equals(part1, part2));
    }
}
