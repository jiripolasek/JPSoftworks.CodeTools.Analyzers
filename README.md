# JPSoftworks.CodeTools.Analyzers

Collection of Roslyn analyzers used in various projects, generalized for re-use in generic projects.

## Rules & Analyzers
| Rule ID                    | Description                                            | Category | Severity       | Notes |
| -------------------------- | ------------------------------------------------------ | -------- | -------------- | ----- |
| [JPX0001](docs/rules/JPX0001.md) | Using `new Guid()` is ambiguous.                       | Design   | ⚠️ Warning      |       |
| [JPX0002](docs/rules/JPX0002.md) | Using `default` literal to create a Guid is ambiguous. | Design   | ℹ️  Information |       |
| [JPX0003](docs/rules/JPX0003.md) | Use `EventArgs.Empty` instead of `new EventArgs()`.    | Usage    | ⚠️ Warning      |       |
| [JPX0004](docs/rules/JPX0004.md) | Use null pattern instead of creating a new empty object. | Usage  | ℹ️  Information |       |
| [JPX0101](docs/rules/JPX0101.md) | Inherited classes must use a suffix.                   | Naming   | ⚠️ Warning      |       |
