# Pastel

A tiny library that colorizes console output by wrapping strings in ANSI escape sequences. It only produces strings; it never writes to the console itself.

This file records the things that aren't obvious from reading the code. Everything else, read the source.


## Target frameworks

`net462;net8.0;net9.0`. Every change has to compile on all three, and the code forks heavily on `#if NET8_0_OR_GREATER` / `#if NET9_0_OR_GREATER`.

**The net462 target compiles as C# 7.3.** The SDK defaults non-Core target frameworks to that language version, and there's no `Directory.Build.props` or `global.json` overriding it. So inside an `#else` branch there are no list patterns, no collection expressions (`[]`), no target-typed `new`. Use `Array.Empty<T>()` rather than `[]` in any code shared across the branches. Verify with:

```powershell
dotnet msbuild src\Pastel.csproj -p:TargetFramework=net462 -getProperty:LangVersion
```

This asymmetry has already produced one shipped crash: the pre-.NET 8 branch hand-rolled what the list pattern does for free, and got it wrong.


## MSBuild: don't condition an ItemGroup on $(DefineConstants)

```xml
<!-- Silently never matches -->
<ItemGroup Condition="$(DefineConstants.Contains('NET8_0_OR_GREATER'))">
```

An `ItemGroup` at the project root is evaluated *before* the framework-specific `DefineConstants` are populated, so `$(DefineConstants)` is only `TRACE`/`TRACE;DEBUG` at that point and the check quietly fails for every target framework. Condition on `$(TargetFramework)` instead.

This shipped broken from 2022 to 2026 without anyone noticing, because a failed condition produces no error — the attribute simply never appeared on any published assembly. `AssemblyTests` now guards it.

The trap is hard to reproduce in isolation: in a *single*-target-framework project the same idiom evaluates to `True`, so a minimal repro will mislead you. Check the real thing instead:

```powershell
Select-String "DisableRuntimeMarshalling" "src\obj\Release\net9.0\Pastel.AssemblyInfo.cs"
```


## Colors: the ConsoleColor web mapper

`s_consoleColorWebMapper` (used by `useWebColors: true`) is the **combined CSS3 list**, which is not the same thing as either the X11 palette or the Windows console palette.

Of the four names where the web and X11 definitions conflict, only two exist in `ConsoleColor`, and both resolve to their **web** value:

| | Pastel | X11 |
| --- | --- | --- |
| `Gray` | #808080 | #BEBEBE |
| `Green` | #008000 | #00FF00 (the web's `Lime`) |

Everything else in the enum is a name the two systems agree on (the `Dark*` shades).

Consequences worth knowing before "fixing" anything:

- **`DarkGray` (#A9A9A9) is lighter than `Gray` (#808080).** `DarkGray` descends from X11, `Gray` from the web, and the combined list keeps both. This is intentional and matches `System.Drawing.Color` exactly — verified for all 15 names it also defines.
- These are **not** the Windows console values (console `Gray` is #C0C0C0 legacy / #CCCCCC Campbell; console `DarkGray` is #808080 / #767676).
- The parameter was called `useLegacy` through v7. `useX11` was considered and rejected: it would contradict both conflicting values.

**The mapper is complete and closed.** It's indexed by `ConsoleColor`, a BCL enum with exactly 16 contiguous members, and all 16 have entries. `Maroon` and `Purple` are the other two web/X11 conflicts but are *not* `ConsoleColor` members, so they can't be added — `ConsoleColor.Maroon` doesn't compile. Anything outside the 16 names goes through the `Color` or hexadecimal overloads.


## The reset scanning invariant

`ScanColorFormatStringInsertPositions` finds every place the color format string has to be re-inserted. The rule, for the tail following each `\x1b[0m`:

> re-insert if the tail is non-empty **and** does not itself start with `\x1b[0m`

A trailing *partial* escape sequence (`\x1b`, `\x1b[`, `\x1b[0`) is not a reset, so it counts as an insert position. That's the case the old net462 branch got wrong: it checked for the `\x1b[` prefix and then sliced 2 more characters that weren't necessarily there.

The predicate was verified exhaustively against the .NET 8 list pattern over 19608 tails (alphabet `\x1b [ 0 m 3 8 x`, lengths 0-5) with zero divergence. If you touch it, re-verify rather than reason about it — the semantics are subtle and the tests are the specification.


## Code style

There is **no `.editorconfig`**; the conventions exist only by example. Match the surrounding code.

- Block-scoped namespace, `using` directives **inside** it, `System` first.
- Allman braces. Single-statement `if` bodies always get braces.
- `#if`/`#else`/`#endif` at **column 0**, never indented.
- Heavy column alignment: `=` signs line up within a block, array initializers align under the opening construct, and operators lead continuation lines (note the padding after `return`).
- Static field prefixes are inconsistent by any standard rule, so match the neighbouring field *of the same shape*: arrays get `s_`, while dictionaries, bools and `char` consts get `_`. Win32 interop constants are `SCREAMING_SNAKE`.


## Line endings

`.gitattributes` sets `* -text`, so git stores and checks out every file byte for byte and converts nothing. That makes the endings entirely the writing tool's responsibility.

The repo is a deliberate mix, and both sides should stay as they are:

- **CRLF** — `.cs`, `.csproj`, `.sln`, `.gitignore` (what Visual Studio writes)
- **LF** — `.github/workflows/*.yml`, `README.md`, `LICENSE`, this file

**Creating a new `.cs` file is the trap.** Anything that writes LF by default will commit an LF source file into a CRLF tree, and nothing will convert it for you — that's what produced the four "Fix line endings" commits, the last of which was cleaning up exactly this mistake. Editing an existing file is safe, since its endings are already there. Check before committing a new one:

```powershell
git ls-files --eol src/YourNewFile.cs   # want i/crlf for sources
```

Sources are also UTF-8 with BOM.


## Tests

xUnit, `net462;net8.0;net9.0`. Nested classes group by concern; methods read `Given_..._Should_...`.

- Parallelization is disabled via an MSBuild `AssemblyAttribute` in `Pastel.Tests.csproj`, not a source attribute. The tests mutate global state (`ConsoleExtensions.Enable()`/`Disable()`, environment variables), so they can't run concurrently.
- Internals are reached through `InternalsVisibleTo`, declared in `src/Pastel.csproj`.
- **A test for a target-framework-specific bug must be run against the unfixed code**, and it should fail on the affected framework only. Several bugs here reproduce on exactly one target framework, so a test that passes everywhere before the fix is testing nothing.

**The test suite is environment-sensitive, and passing locally proves little.** Pastel disables itself in a CI/CD environment, so anything asserting on colored output fails on a build server unless it's in the `ColorOutputEnabledCollection`, whose fixture calls `Enable()`. `EnvironmentTests` has the mirror problem: it enumerates every variable of the process, so a real `CI` or `GITHUB_ACTION` variable is detected alongside the one under test — it suppresses the known ambient names and restores them afterwards.

Check a change against all three before trusting it:

```powershell
dotnet test -c Release                                    # local
$env:CI = 'true';     dotnet test -c Release; Remove-Item Env:\CI
$env:NO_COLOR = '1';  dotnet test -c Release; Remove-Item Env:\NO_COLOR
```

This bug existed from the beginning and stayed invisible because the release workflow had never once run. Its first run failed with 60/114/8 failures across the three target frameworks — differing counts for identical code, which looks like a race but isn't; it's just test ordering deciding who last touched the shared `_enabled` flag.


## Generated files

`src/Pastel.xml` is **not tracked**. Every build regenerates it, and `dotnet pack` picks it up from the build output into `lib/{tfm}/Pastel.xml`. Don't commit it.

All three target frameworks write that same path, so the file is whichever one built last. It's identical across them today, which is the only reason it doesn't matter.


## Release

`.github/workflows/release.yml`, `workflow_dispatch` only, pushes to NuGet via Trusted Publishing.

**It must run on `windows-latest`.** The Ubuntu runners have neither Mono nor the .NET Framework reference assemblies, so net462 can't be built or tested there. The workflow had never been run even once before this was corrected, so nothing had ever surfaced it. Never trigger it without asking — it publishes.


## Performance

The hot path is string building, so allocation matters more than cleverness. Allocations per call on net9.0, which are stable and worth not regressing:

| | |
| --- | --- |
| `Pastel(Color)` | 96 B |
| `Pastel(ConsoleColor)` | 64 B |
| `Pastel(hex)` | 96 B |
| `Pastel(Color)`, 1 embedded reset | 184 B |

Roughly 55-90 ns/call, but timings swing by ~15% between runs on the same code — don't trust a single measurement, and don't read a small delta as a win.

Two lessons from the last round of tuning:

- `Pastel(ConsoleColor)` used to be the slowest overload despite doing the least work, purely because of a `ReadOnlyDictionary` lookup (10.07 ns vs 0.37 ns for an array index). It's now the fastest.
- **Micro-benchmark results don't always transfer.** `new int[0]` measures as 0 B/op in a tight loop because the JIT elides it, yet replacing it with `Array.Empty<int>()` at the real call site cut 24 B/op from three overloads. Measure at the call site.


## Commit style

Imperative, sentence case, no `feat:`/`fix:` prefixes, no trailing period. Multi-change commits list the extras as `*` bullets after the subject, then explain *why* in prose. Reference issues inline (`Fixes #42`). **Never** add a `Co-Authored-By` trailer or any model attribution.
