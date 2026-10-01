# Changelog

## [2.0.0](https://github.com/Mehrdad32/DNSetter/releases/tag/v2.0.0) — 2026-10-01

### Added

- Explicit network adapter selection and a copyable view of current IPv4 DNS.
- Separate Core and Windows platform projects, with 17 regression scenarios for validation, adapter identity, verification, rollback and serialized changes.
- A resizable native Windows Forms interface, keyboard shortcuts, persistent operation status, high-contrast colors and PerMonitorV2 DPI support.
- A native UI smoke check using simulated networking, including compact-window and large-text captures.
- Persian and English documentation, contribution guidance, issue/PR templates and an MIT license file.
- Self-contained x64/x86 release packages with SHA-256 checksums.

### Changed

- DNS writes target the selected adapter using its stable ID and a freshly read interface index.
- Changes are verified; failed writes or verification attempt to restore the prior configuration of that adapter.
- Automatic DNS explicitly selects automatic IPv4 DNS, without changing IP addresses, gateways or IPv6 DNS.
- Ping runs asynchronously, with independent requests, progress and cancellation when the results window closes.
- Site reachability reports the HTTP result without claiming that a DNS preset bypasses restrictions.
- CI validates Core, Windows publishing and the native UI before release publication.

### Fixed

- Google's built-in secondary DNS is `8.8.4.4`. Only the exact legacy built-in pair is migrated; custom entries are preserved.
- Duplicate DNS addresses are removed and invalid IPv4 input cannot be applied.
- Preset selection and edited input are kept separate from the current adapter DNS display.
- Duplicate preliminary ping passes and shared `Ping` instances were removed.

## [1.0.1.2](https://github.com/Mehrdad32/DNSetter/releases/tag/v1.0.1.2) — 2025-07-05

- Previous public Windows x64/x86 release. See the release page for the original assets.
