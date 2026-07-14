namespace SolutionName.Abstractions.DataModels;

using SolutionName.Abstractions.Generation;

/// <summary>
/// Spike demonstration for web-template#79. A data contract whose concrete is
/// generated: it carries scalar properties and a covariant read-only collection
/// (the case flagged as fiddly to hand-write). The concrete lives in
/// SolutionName.DataModels — chosen purely by where the partial stub is declared.
/// Remove this pair when converting real models to the pattern.
/// </summary>
[GenerateConcrete]
public interface ISampleContract
{
    string Name { get; set; }

    int Count { get; set; }

    IReadOnlyList<string> Tags { get; }
}
