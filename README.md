# JPSoftworks.CodeTools.Analyzers

Collection of Roslyn analyzers used in various projects, generalized for re-use in generic projects.

## Rules & Analyzers
| Rule ID                         | Description                                                    | Category | Severity      | Status | Notes |
| ------------------------------- | -------------------------------------------------------------- | -------- | :------------ | :----: | ----- |
| [JPX0001](doc/rules/JPX0001.md) | Using `new Guid()` is ambiguous.                               | Design   | ⚠️ Warning     |   ✅    |       |
| [JPX0002](doc/rules/JPX0002.md) | Using `default` literal to create a Guid is ambiguous.         | Design   | ℹ Information |   ✅    |       |
| [JPX0003](doc/rules/JPX0003.md) | Use `EventArgs.Empty` instead of `new EventArgs()`.            | Usage    | ⚠️ Warning     |   🚀    |       |
| [JPX0004](doc/rules/JPX0004.md) | Use null-objectpattern instead of creating a new empty object. | Usage    | ℹ️ Information |   🚀    |       |
| [JPX0101](doc/rules/JPX0101.md) | Inherited classes must use a suffix.                           | Naming   | ⚠️ Warning     |   🚀    |       |

## Wishlist
| Rule ID                         | Description                                                                      | Category | Severity  | Status | Notes |
| ------------------------------- | -------------------------------------------------------------------------------- | -------- | :-------- | :----: | ----- |
| [JPX0005](doc/rules/JPX0005.md) | Use `EventHandler` delegate for events.                                          | Design   | ⚠️ Warning |   💡    |       |
| [JPX0006](doc/rules/JPX0006.md) | Use `EventArgs.Empty` instead of `null`.                                         | Design   | ⚠️ Warning |   💡    |       |
| [JPX0007](doc/rules/JPX0007.md) | Use correct sender in event invocation (`this` for instance, `null` for static). | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0008](doc/rules/JPX0008.md) | Shouldn't call `Type.GetType()`, probably meant GetType() on object instance.    | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0009](doc/rules/JPX0009.md) | Shouldn't return null from async method.                                         | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0021](doc/rules/JPX0021.md) | Use String.Equals instead of == operator.                                        | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0022](doc/rules/JPX0022.md) | Use String.Equals instead of String.Compare(...) == 0.                           | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0023](doc/rules/JPX0023.md) | Specify case comparison explicitly when comparing strings.                       | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0024](doc/rules/JPX0024.md) | Specify comparer when creating collection of strings.                            | Usage    | ⚠️ Warning |   💡    |       |
| [JPX0025](doc/rules/JPX0025.md) | String literal contains zero-width character.                                    | Usage    | ⚠️ Warning |   💡    |       |

### Status legend:
- 💡: idea / wishlisted
- 🧪: design
- 🚀: implementation
- 💚: implementation + unit test
- ✅: implementation + unit test + documentation