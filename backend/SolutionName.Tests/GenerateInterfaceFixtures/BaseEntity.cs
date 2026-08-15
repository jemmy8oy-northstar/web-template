using SolutionName.Abstractions.Generation;

namespace SolutionName.Tests.GenerateInterfaceFixtures;

/// <summary>A mirrored base class — see <see cref="DerivedEntity"/>.</summary>
[GenerateInterface]
public class BaseEntity : IBaseEntity
{
    public string Id { get; set; } = "";
}
