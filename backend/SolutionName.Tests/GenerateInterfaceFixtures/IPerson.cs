namespace SolutionName.Tests.GenerateInterfaceFixtures;

/// <summary>
/// The route-facing contract interface. It closes the generic over CONCRETE
/// types, not interfaces — a class whose property is <c>Addr</c> does not
/// implement an interface declaring <c>IAddr</c>, and System.Text.Json cannot
/// deserialise into an interface.
/// </summary>
public interface IPerson : IPersonBase<Addr>
{
}
