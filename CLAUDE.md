# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release

# Run tests in a specific project
dotnet test Tests.XUnit --configuration Release

# Run a single test by name
dotnet test Tests.XUnit --filter "FullyQualifiedName~TestMethodName"

# Pack NuGet packages (VERSION comes from git tag, e.g. v3.0.0 → 3.0.0)
dotnet pack --configuration Release /p:Version=$VERSION
```

## Architecture

DepenMock is a test helper library that combines AutoFixture with a pluggable mocking framework to auto-mock dependencies in unit tests. Every project multi-targets `net8.0` and `net10.0`. It is split into separate NuGet packages.

### Core (`DepenMock/`)

- **`Container`** — the central class. Wraps AutoFixture's `IFixture` and manages mock instances. `Resolve<T>()` builds SUT instances with all dependencies auto-mocked; `ResolveMock<T>()` retrieves (or creates and caches) an `IMock<T>` for a specific dependency.
- **`IMock<T>` / `IMockFactory`** — abstractions that decouple the core from any specific mocking library. Each mocking-framework adapter implements `IMockFactory` to produce `IMock<T>` wrappers.
- **`BaseTest` / `BaseTestByAbstraction<,>` / `BaseTestByType<>`** — base test classes (provided per test-framework adapter) with a `Container` property and a virtual `AddContainerCustomizations()` hook.
- **`ListLogger<T>`** — in-memory `ILogger<T>` implementation for asserting log output in tests.
- **`LogOutputAttribute`** / **`LogOutputHelper`** — attribute (with a `LogOutputTiming` of `Always` / `OnFailure` / `OnSuccess`) that pipes log output to the test runner's output, plus the shared helper that decides whether to emit and formats the messages. The `DepenMock.NUnit`, `DepenMock.MSTest`, and `DepenMock.XUnit` base classes honor this core attribute. `DepenMock.XUnit.V3` ships its **own** `LogOutputAttribute` that must be used instead — only xUnit v3 exposes test outcome (via `IBeforeAfterTestAttribute`), which the timing modes need.

### Mocking-framework adapters

| Package | Key types |
|---|---|
| `DepenMock.Moq` | `MoqMockFactory`, `MoqMock<T>`, `.AsMoq()` extension |
| `DepenMock.NSubstitute` | `NSubstituteMockFactory`, `NSubstituteMock<T>`, `.AsNSubstitute()` extension |
| `DepenMock.FakeItEasy` | `FakeItEasyMockFactory`, `FakeItEasyMock<T>`, `.AsFake()` extension |

AutoFixture glue (`AutoMoqCustomization` / `AutoNSubstituteCustomization` / `AutoFakeItEasyCustomization`) is wired inside each factory.

Each adapter also defines a `SetupMock<T>()` extension method on `Container` that resolves the mock, applies an inline configuration action written against that framework's native mock type, and returns the `Container` for chaining. It shares the same mock cache as `ResolveMock<T>()`.

### Test-framework adapters

`DepenMock.NUnit`, `DepenMock.XUnit`, `DepenMock.XUnit.V3`, `DepenMock.MSTest` each expose framework-specific base classes that inherit from the core base classes and pass the `IMockFactory` to `Container`. The mock factory is always an explicit constructor parameter (introduced in v3.0.0 — it was an implicit Moq default before).

### Sample / test projects

`DeskBooker.Core` is a sample domain model used only by the six `Tests.*` projects to demonstrate real usage of each adapter combination. It is not a deployable package.

## Package publishing

Releases are triggered by pushing a `v*` git tag. All eight NuGet packages are versioned identically. See `CONTRIBUTING.md` for the exact tagging steps and `RELEASES.md` for breaking-change history.
