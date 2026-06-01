# .NET Version Upgrade

## Strategy
**Selected**: All-at-Once — upgrade all 5 projects simultaneously in a single atomic pass.
**Rationale**: 5 projects, all net8.0, ≤3-tier depth, no CI-green constraint.

### Execution Constraints
- All projects are updated in a single pass — no phased ordering or tier sequencing
- Full solution build validation after all project and package changes are applied
- API breaking changes (Api.0001, Api.0002) fixed inline — no stubs or deferrals
- `Microsoft.NET.Sdk.Functions` must be replaced with isolated worker packages before building
- CPM (`Directory.Packages.props`) to be set up as part of the upgrade task

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0 (.NET 10.0 LTS)
- **Commit Strategy**: Manual (no automatic commits)

## Upgrade Options
- **Strategy**: All-at-Once
- **Package Management**: Central Package Management (CPM)
- **Unsupported API Handling**: Fix Inline

## Source Control
- **Source Branch**: features/696004_netupgrade
- **Working Branch**: features/696004_netupgrade (no new branch)
