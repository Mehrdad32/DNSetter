# Contributing to DNSetter

Bug reports and focused pull requests are welcome. Start with the [issue templates](https://github.com/Mehrdad32/DNSetter/issues/new/choose), include reproduction steps, and avoid posting credentials or private network details in public logs.

## Setup

Use Windows, Git and the .NET 8 SDK to work on the full app. The Core and its regression runner also work on Linux.

```powershell
git clone https://github.com/Mehrdad32/DNSetter.git
cd DNSetter
dotnet build DNSetter.sln -c Release
dotnet run --project tests/DNSetter.Core.Tests/DNSetter.Core.Tests.csproj -c Release
```

Run `bin\Release\net8.0-windows\DNSetter.exe` directly for manual testing. Its manifest requests Administrator privileges.

## Project layout

| Location | Responsibility |
| --- | --- |
| `DNSetter.Core/` | DNS models, IPv4 validation, change verification and rollback; no WinForms or Windows commands |
| `DNSetter.Windows/` | Adapter/registry reads, privilege checks and `netsh`/cache-flush execution |
| Root WinForms project | Adapter selection, current vs. pending DNS, presets and diagnostic UI |
| `tests/DNSetter.Core.Tests/` | Package-free regression runner with a fake platform |
| `tests/DNSetter.Ui.Smoke/` | Native UI automation and screenshots with a fake adapter |
| `docs/` | Manual test guide, screenshots and release notes |

## Checks

The Core regression runner uses **`dotnet run`, not `dotnet test`**. Neither it nor the UI smoke check changes real network settings.

```powershell
dotnet run --project tests/DNSetter.Core.Tests/DNSetter.Core.Tests.csproj -c Release
dotnet run --project tests/DNSetter.Ui.Smoke/DNSetter.Ui.Smoke.csproj -c Release -- ui-preview
```

The UI check requires Windows. CI runs the Core tests on Linux, publishes both Windows architectures and runs the native UI check on x64. Manual checks of real adapter writes, multiple adapters and display scaling are documented in [docs/testing.fa.md](docs/testing.fa.md).

## Pull requests

- Branch from `master`; use a descriptive branch name.
- Keep the change focused and explain the user-visible behavior and validation.
- Preserve explicit adapter selection and the distinction between configured and effective DNS.
- Keep platform commands out of Core. Construct process arguments separately and check exit codes.
- Add a regression scenario when fixing behavior that can fail independently; avoid tests that only repeat the implementation.
- Update both READMEs and the changelog when behavior or controls change.
- Follow `.editorconfig`; exclude build output and personal `list.json` files.

## Build portable packages

```powershell
dotnet publish DNSetter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o publish/win-x64
dotnet publish DNSetter.csproj -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o publish/win-x86
```

## Release process

1. Update `<Version>` in `DNSetter.csproj` to a stable `major.minor.patch` version. Keep assembly/file versions consistent with the release line.
2. Add `docs/release-notes/v<version>.md`, update `CHANGELOG.md`, and update the version and download links in both READMEs.
3. Pass CI and manually verify the Windows app, then merge into `master`.
4. The Release workflow detects the new version, runs Core/UI checks, builds x64/x86 ZIPs, generates `SHA256SUMS.txt`, and creates the matching tag and public GitHub release at that commit.

An existing release is skipped. The workflow also accepts a matching `v*` tag or a manual dispatch; publication by manual dispatch is limited to `master`. Only the publication job receives repository write permission.
