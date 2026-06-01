# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade all 5 projects from net8.0 to net10.0
**Scope**: Small solution — 2 source projects + 3 test projects, all SDK-style on modern .NET

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 5 projects, all on net8.0, ≤3-tier dependency depth — no phasing needed.

## Tasks

### 01-prerequisites: Verify SDK and toolchain for net10.0

Confirm the .NET 10 SDK is installed and any `global.json` version constraints are compatible with net10.0. This is a fast gate check before making any project changes.

**Done when**: .NET 10 SDK confirmed installed; `global.json` (if present) updated to allow net10.0 builds.

---

### 02-upgrade-all-projects: Upgrade all projects to net10.0

Upgrade all 5 projects simultaneously:
- `src/Defra.Trade.Events.EHCO.GCSubscriber` — Azure Functions host project. This is the highest-risk project: `Microsoft.NET.Sdk.Functions` (in-process) must be removed and replaced with the isolated worker packages (`Microsoft.Azure.Functions.Worker`, `Microsoft.Azure.Functions.Worker.Sdk`, `Microsoft.Azure.Functions.Worker.Extensions.Http`). This project also has 3 binary-incompatible (`Api.0001`) and source-incompatible (`Api.0002`) API issues to resolve inline, plus `System.Net.Http` (redundant — included in the framework) to remove.
- `src/Defra.Trade.Events.EHCO.GCSubscriber.Application` — Class library. `AutoMapper` has a security vulnerability and must be updated to 16.1.1. `Microsoft.Extensions.*` packages should be updated to 10.x.
- `test/Defra.Trade.Events.EHCO.GCSubscriber.Tests.Common` — Test helper library. `AutoMapper` security vulnerability, deprecated `xunit` and `Azure.Identity` packages.
- `test/Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests` — Unit tests for Application layer. Deprecated `xunit` package.
- `test/Defra.Trade.Events.EHCO.GCSubscriber.UnitTests` — Unit tests for the main function app. Source-incompatible API changes (`Api.0002`) to fix inline.

All projects get their `<TargetFramework>` updated to `net10.0`. Package references are updated. API breaking changes are fixed inline. A `Directory.Packages.props` file is created to centralise package versions (CPM).

**Done when**: All 5 projects target net10.0; solution builds with 0 errors and 0 warnings; all tests pass; `Directory.Packages.props` is in place with versions removed from individual project files.

---

### 03-final-validation: Final solution validation

Run the full test suite and confirm the solution is clean. Document any deferred recommendations (e.g., reviewing deprecated `Azure.Identity` and `xunit` replacements if not handled in task 02).

**Done when**: All tests pass; no build errors or warnings; execution log updated with final status.
