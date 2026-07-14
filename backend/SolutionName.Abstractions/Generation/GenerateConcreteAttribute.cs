namespace SolutionName.Abstractions.Generation;

/// <summary>
/// Marks an interface whose trivial data-property implementation should be
/// generated onto any <c>partial class</c> that implements it.
///
/// Author the interface (the contract) here in Abstractions; declare a one-line
/// partial stub in the project that should own the concrete
/// (<c>public partial class Foo : IFoo;</c>). The
/// <c>ConcreteFromInterfaceGenerator</c> fills the property boilerplate — the
/// interface stays the single source of truth and the concrete's location is
/// chosen by where the stub is written.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class GenerateConcreteAttribute : Attribute;
