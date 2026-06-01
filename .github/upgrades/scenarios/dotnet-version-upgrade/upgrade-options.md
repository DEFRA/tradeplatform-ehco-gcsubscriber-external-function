# Upgrade Options — Defra.Trade.Events.EHCO.GCSubscriber

Assessment: 5 projects, all net8.0 → net10.0, API incompatibilities in 2 projects, security vulnerabilities, `Microsoft.NET.Sdk.Functions` needs replacing with isolated worker packages

## Strategy

### Upgrade Strategy
All projects are on net8.0 (modern .NET), 5 projects, ≤3-tier dependency depth, ≤2 high-risk API issues — All-at-Once is the right fit.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all 5 projects simultaneously in a single atomic pass. Fastest approach — no multi-targeting overhead. Solution may be temporarily broken until all projects are updated. |
| Top-Down | Upgrade the main function app first, temporarily multi-targeting shared libraries; consolidate in a second phase. More overhead, not needed at this scale. |

---

## Project Structure

### Package Management
5 projects, all SDK-style, modern-to-modern upgrade (net8.0 → net10.0), no `Directory.Packages.props` detected.

| Value | Description |
|-------|-------------|
| **Central Package Management (CPM)** (selected) | Create `Directory.Packages.props`, move package versions out of project files. All projects are SDK-style and on the same ecosystem — CPM applies cleanly without friction. |
| Per-Project (defer CPM) | Each project retains its own versions. Defer CPM as a post-migration task. |

---

## Compatibility

### Unsupported API Handling
API incompatibilities detected: 3 binary-incompatible (`Api.0001`) and 13 source-incompatible (`Api.0002`) occurrences across `GCSubscriber` and `GCSubscriber.UnitTests`.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same upgrade task, including complex ones. Modern-to-modern upgrade — changes are expected to be minor. No deferred stubs. |
| Defer Complex Changes | Apply simple replacements inline; generate stubs for complex changes and create resolution subtasks. |
