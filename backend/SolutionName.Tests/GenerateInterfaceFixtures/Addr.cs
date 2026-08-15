using SolutionName.Abstractions.Generation;

namespace SolutionName.Tests.GenerateInterfaceFixtures;

/// <summary>A leaf: nothing about it varies between layers, so no generic base.</summary>
[GenerateInterface]
public class Addr : IAddressLike, IAddr
{
    public string Line1 { get; set; } = "";
}
