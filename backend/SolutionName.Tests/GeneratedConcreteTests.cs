using SolutionName.Abstractions.DataModels;
using SolutionName.DataModels.Models;

namespace SolutionName.Tests;

/// <summary>
/// Proves ConcreteFromInterfaceGenerator (web-template#79) materialised the whole
/// property implementation of <see cref="ISampleContract"/> onto the hand-written
/// one-line partial stub <see cref="SampleContract"/> — scalars round-trip and the
/// covariant read-only collection is generated non-null and settable.
/// </summary>
public class GeneratedConcreteTests
{
    [Fact]
    public void Generated_concrete_implements_the_contract()
    {
        ISampleContract contract = new SampleContract
        {
            Name = "widget",
            Count = 3,
            Tags = ["a", "b"],
        };

        Assert.Equal("widget", contract.Name);
        Assert.Equal(3, contract.Count);
        Assert.Equal(new[] { "a", "b" }, contract.Tags);
    }

    [Fact]
    public void Generated_collection_property_defaults_to_empty_not_null()
    {
        var contract = new SampleContract();

        Assert.NotNull(contract.Tags);
        Assert.Empty(contract.Tags);
    }
}
