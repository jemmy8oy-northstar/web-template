namespace SolutionName.Tests.GenerateInterfaceFixtures;

/// <summary>
/// Hand-written, because it is the CONSTRAINT the generic layering hangs on:
/// `where TAddress : IAddressLike` is what lets one consumer serve both the
/// contract and the domain model.
/// </summary>
public interface IAddressLike
{
    string Line1 { get; set; }
}
