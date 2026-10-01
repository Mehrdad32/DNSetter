# DNSetter

[فارسی](README.md) · [Download v2](https://github.com/Mehrdad32/DNSetter/releases/tag/v2.0.0) · [Report a bug](https://github.com/Mehrdad32/DNSetter/issues/new/choose)

[![CI](https://github.com/Mehrdad32/DNSetter/actions/workflows/core-ci.yml/badge.svg?branch=master)](https://github.com/Mehrdad32/DNSetter/actions/workflows/core-ci.yml)
[![Release](https://img.shields.io/github/v/release/Mehrdad32/DNSetter)](https://github.com/Mehrdad32/DNSetter/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

**DNSetter** is a portable Windows app for viewing, testing and changing **IPv4 DNS on a selected network adapter**, with saved DNS presets.

![DNSetter main window](docs/screenshots/main.png)

*Captured from the app with sample network data.*

## Download and run

Stable version: **2.0.0** — Windows 10 / 11. The packages include .NET; a separate runtime installation is not required.

| System | Download |
| --- | --- |
| 64-bit Windows | [DNSetter-win-x64.zip](https://github.com/Mehrdad32/DNSetter/releases/download/v2.0.0/DNSetter-win-x64.zip) |
| 32-bit Windows | [DNSetter-win-x86.zip](https://github.com/Mehrdad32/DNSetter/releases/download/v2.0.0/DNSetter-win-x86.zip) |
| File checksums | [SHA256SUMS.txt](https://github.com/Mehrdad32/DNSetter/releases/download/v2.0.0/SHA256SUMS.txt) |

Extract the appropriate ZIP to a writable folder and run `DNSetter.exe`. The app requests **Administrator** access to change DNS. Presets are stored in `list.json` beside the executable; keep this file when upgrading.

## Usage

1. Select the network adapter to configure. If several adapters are connected, choose one explicitly.
2. Read **Current IPv4 DNS**, then select a preset or enter your own addresses. The primary address is required; the secondary address is optional.
3. Click **Apply DNS**. The app writes to that adapter and checks the result reported by Windows.
4. Click **Automatic DNS** to use DNS provided automatically by the network.

**Automatic DNS selects automatic (DHCP) DNS; it does not restore an earlier manual configuration.** To restore a manual configuration after a successful change, enter its original addresses and apply them again. If a change fails, DNSetter attempts to restore the configuration from immediately before that operation.

## Controls

| Control | Behavior |
| --- | --- |
| Refresh | Reload adapters and the selected adapter's current DNS |
| Apply DNS | Apply one or two valid IPv4 addresses to the selected connected adapter, verify the result and flush the DNS cache |
| Automatic DNS | Set the selected adapter's IPv4 DNS to automatic |
| Save preset | Save or update the input addresses in `list.json` without changing network settings |
| Ping DNS | Send ICMP pings to the input addresses and report response times or status |
| Test all presets | Test presets concurrently with progress and a results table; closing the window cancels remaining work |
| Adapter details | Show the adapter ID, interface index, configuration and effective IPv4/IPv6 DNS |
| Site reachability | Check Gemini's HTTPS response through Windows network/proxy settings |

![Ping results](docs/screenshots/results.png)

*The displayed results are examples, not a comparison of DNS providers.*

## Scope

- Only the selected adapter's **IPv4 DNS** is written. IP addresses, gateways, other adapters and IPv6 DNS are left intact.
- **Ping does not test DNS resolution.** A DNS server can work while blocking ICMP.
- Site reachability reports an HTTP response; it does not establish that a preset bypasses filtering or geographic restrictions.
- Browser Secure DNS / DoH, VPNs and network policies can use a different resolver.
- Rollback applies to failed operations. There is no persistent recovery history for a power loss or forced termination.

## Troubleshooting

| Symptom | Check |
| --- | --- |
| Apply is disabled | Select a connected adapter and enter a valid primary IPv4 address |
| An adapter is missing | Check the connection and refresh; only IPv4-capable adapters are listed |
| Access denied while changing DNS | Run with Administrator privileges |
| Preset saving fails | Move the app to a folder where it can write `list.json` |
| Ping times out | ICMP may be blocked by the network or server |
| Automatic shows no DNS server | Automatic DNS depends on network/DHCP configuration; static-IP networks may provide none |

When opening an [issue](https://github.com/Mehrdad32/DNSetter/issues), include the app version, Windows version, x64/x86 architecture, reproduction steps and complete error message.

## Development

Built with **C#, .NET 8 and Windows Forms**. The Core is independent of the UI and Windows command execution.

```powershell
dotnet build DNSetter.sln -c Release
dotnet run --project tests/DNSetter.Core.Tests/DNSetter.Core.Tests.csproj -c Release
```

Full builds require Windows and the .NET 8 SDK. Core tests run on Windows or Linux without changing real network settings. The test runner uses `dotnet run`.

[Contributing and publishing](CONTRIBUTING.md) · [Windows manual checks (Persian)](docs/testing.fa.md) · [Changelog](CHANGELOG.md)

## License

Project code is available under the [MIT license](LICENSE).
