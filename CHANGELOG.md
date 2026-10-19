# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-10-19

### Added

- Harmony patch on `ExploreManager.set_ExposureValue(int)` so every write to the
  exposure value is scaled, regardless of its source
- Fallback patch on `ExploreManager.AddExposure(int)` for the action-driven growth path
- `General.ExposureGrowthRate` config entry (`0`–`1`, default `0` = frozen)
- Startup logging that reports which members were patched, or which one is missing
  after a game update

[Unreleased]: https://github.com/gzzheng/survival-log-noexposure/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/gzzheng/survival-log-noexposure/releases/tag/v1.0.0
