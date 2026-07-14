# Concrete-from-interface source generator (spike — web-template#79)

**Status:** proof-of-concept, awaiting a direction call from James.
**Context threads:** web-template#79, snip-it#5, whos-accountable#12.

## The problem

The layered template uses **interface-only-in-abstractions**: every data carrier a
DI-registered service can return is an `interface` in `Abstractions`, with the
`concrete` in `DataModels`/`DomainModels`. This guarantees a service can never hand a
concrete straight to a route un-mapped ([backend-architecture.md](backend-architecture.md)).

The cost: the concrete re-declares every member of the interface by hand. Add a
property to `IStatus` and forget it on `Status` and the compiler only catches it if the
member is used. For collections the hand-written plumbing is fiddlier still. This is
pure boilerplate that wants generating.

## Recommendation: **roll our own, generate the concrete _from_ the interface**

### Build vs. buy
Evaluated the field. Community generators (AutoInterface / AutoDeriveInterface and
similar) almost all generate the **interface from a concrete** — the wrong direction
for us (see below) — and none target our specific covariant-collection shape. Metalama
does more but is a heavier dependency with licensing to weigh. Our shape is narrow and
specific, so a ~150-line `IIncrementalGenerator` with **no third-party runtime
dependency** is the cleaner long-term answer. That's what this spike builds:
`SolutionName.SourceGenerators`.

### Why generate the concrete from the interface, not the reverse

James's question on #79:

> "generator interface-from-concrete could work better cus then we can do it for domain
> models as well. However, then the interfaces won't be in the abstraction project right"

That second sentence is exactly the deciding constraint. **A Roslyn source generator can
only emit code into the compilation it runs in — it cannot place generated code in a
different project.** So:

| | Generate **concrete** from interface (chosen) | Generate **interface** from concrete |
|---|---|---|
| Hand-authored half | the interface (the contract) | the concrete |
| Generated half lands in | the project that holds the concrete | the project that holds the **concrete** |
| Interface ends up in `Abstractions`? | **Yes** — you author it there | **No** — it's emitted next to the concrete, in the wrong assembly |
| Preserves the DI-seam guarantee | **Yes** | No — breaks the layering |
| Covariant `IReadOnlyList<IFoo>` handling | trivial — concrete mirrors the interface type | must synthesise covariance from `List<Foo>` |

Generating the interface from the concrete would trap the interface in
`DataModels`/`DomainModels`, so `Services`/`WebApi` (which reference only `Abstractions`)
couldn't see it. James spotted this himself. Generating the **concrete** keeps the
interface hand-authored in `Abstractions` as the single source of truth, and — because
the concrete simply mirrors the interface's declared types — the covariant-collection
case that makes the reverse direction fiddly just disappears.

### "…so we can do it for domain models as well"
Direction 1 covers domain models too, without a second mechanism. A behaviour-carrying
domain model is authored as its interface (data **and** method signatures) in
`Abstractions`, plus a partial concrete you write for the behaviour bodies. The
generator fills only the **data-property** half it can see is missing; anything you or a
base class already provide is left alone. `DomainStatus : Status, IDomainStatus` needs
nothing generated (Status supplies the data, you supply `GetFriendlyStatus`).

## How it works (the PoC)

The location of the concrete is chosen by **where you declare a one-line partial stub** —
that is what routes generated code to the right project (`DataModels` vs `DomainModels`),
solving the cross-project concern.

1. Mark the interface in `Abstractions`:
   ```csharp
   [GenerateConcrete]                         // Abstractions/Generation/GenerateConcreteAttribute.cs
   public interface ISampleContract
   {
       string Name { get; set; }
       int Count { get; set; }
       IReadOnlyList<string> Tags { get; }
   }
   ```
2. Declare the concrete's home + name with a one-line stub in the owning project:
   ```csharp
   // SolutionName.DataModels/Models/SampleContract.cs — the whole hand-written concrete
   public partial class SampleContract : ISampleContract;
   ```
3. The generator emits the companion partial:
   ```csharp
   partial class SampleContract
   {
       public string Name { get; set; } = default!;
       public int Count { get; set; }
       public IReadOnlyList<string> Tags { get; init; } = [];
   }
   ```

Get-only interface members become `{ get; init; }`; get/set stay `{ get; set; }`.
Collections default to `[]`; non-nullable reference scalars get `= default!;` to satisfy
CS8618. Members already declared by the author or inherited from a base class are skipped.

Wired in via an analyzer reference (`OutputItemType="Analyzer"
ReferenceOutputAssembly="false"`) — see `SolutionName.DataModels.csproj`. Proven by
`GeneratedConcreteTests` (scalars round-trip, the collection is generated non-null).

## Trade-offs / open points for review

- **Semantic defaults are lost.** The hand-written `Status` seeded `Version = "1.0.0"`; a
  generated concrete can't know that. Where a model needs seed values, keep the property
  hand-written (the generator skips author-declared members) or set them at construction.
  This is why the PoC demonstrates on a fresh `ISampleContract`/`SampleContract` pair
  rather than converting `Status` — conversion is the rollout step, per model, once the
  direction is agreed.
- **One generator, many consumers.** Each project that owns concretes references the
  generator as an analyzer; the stub's location does the routing. No assembly-level
  registration needed.
- **Rollout** (after approval): convert the hand-written concretes across the org repos
  that carry this boilerplate — snip-it, whos-accountable, language-vocab,
  holiday-planning, habits — and delete the `ISampleContract`/`SampleContract` demo pair.
