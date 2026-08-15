namespace SolutionName.Tests.GenerateInterfaceFixtures;

/// <summary>The domain view: same generic base, narrowed to domain types.</summary>
public interface IDomainPerson : IPersonBase<DomainAddr>
{
}
