# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Removed

## [1.0.6] - 2024-09-08

### Fixed

- Fix IntgerColumn sorting 

## [1.0.5] - 2024-08-22

### Added

- Added DatePickerWidget, DateRangePickerWidget, TimeSpanPickerWidget
- Added DateRangeFormField, TimeSpanFormField

### Changed

- DateFormField now uses the DatePickerWidget

## [1.0.4] - 2024-08-20

### Added

- Added FlagsEnumFormField

### Changed

- Reworked FormFields to have Input,Label,Help and Draw as individual functions allowing each to be used individually


## [1.0.3] - 2024-08-19

### Added

- FormFields can have their label and input size specified when drawing
- Settings renamed to FormFields
- Grid system added that utilizes FormFields/Columns

### Fixed

- UniversalisListing had lastReviewTime as a int and not a long


## [1.0.2] - 2024-08-08

### Added

- Fix CSV skipping the first row
- Fix the wizard showing scrollbars where the image is

## [1.0.1] - 2024-08-07

### Added

- Wizard,VerticalSplitter added to AllaganLib.Interface
- Added AllaganLib.Data for CSV saving/loading

## [1.0.0] - 2024-08-04

### Added

- Initial release providing AllaganLib.Interface, AllaganLib.Universalis, AllaganLib.Shared

