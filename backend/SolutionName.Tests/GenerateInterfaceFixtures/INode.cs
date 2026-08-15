namespace SolutionName.Tests.GenerateInterfaceFixtures;

/// <summary>Closes the self-referential base on the concrete node type.</summary>
public interface INode : INodeBase<Node>
{
}
