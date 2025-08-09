# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Removed+

## [1.3.2] - 2025-08-09

### Changed
- Use explicit PackageReference to Microsoft.Extensions.Logging.Abstractions

## [1.3.1] - 2025-08-09

### Changed
- Use correct dependencies
- Update LuminaSupplemental to 3.0.1

## [1.3.0] - 2025-08-07

### Changed
- Initial 7.3 support
- Bumped dependencies

- AllaganLib.GameSheets
  - LuminaSupplemental updated to 3.0.0


## [1.2.17] - 2025-06-19

### Changed
- Bumped dependencies

- AllaganLib.GameSheets
  - LuminaSupplemental updated to 2.3.12

### Fixed
- AllaganLib.Interface
  - RenderTables now provide access to IsDirty
  - Sorting a RenderTable will now cause a refresh of the table's data


## [1.2.16] - 2025-06-12

### Added
- AllaganLib.Interface
  - HotkeyService added
  - HotkeyFormField added
- AllaganLib.Shared
  - HostedFrameworkService added 

## [1.2.15] - 2025-06-05

### Added
- AllaganLib.GameSheets
  - PVP Series source added
  - Quests now pull in extra required items parsed from the lua in the game
  - Field Op coffers/treasure sources added
  - Collectable Shop source added
  - Deep Dungeon sources added

### Changed
- AllaganLib.GameSheets
  - Updated LuminaSupplemental.Excel to 2.3.7
  - Reworked the Items/CostItems sources provide, they now include quantity, hq, probability, min and max if applicable
  - Certain sources now provide more comprehensive requirement/reward information

## [1.2.14] - 2025-05-27

### Added
- AllaganLib.GameSheets
  - Updated LuminaSupplemental.Excel to 2.3.3

## [1.2.13] - 2025-05-21

### Added
- AllaganLib.Interface
  - RenderTable's now provide a IsLoading flag

## [1.2.12] - 2025-05-18

### Fixed
- AllaganLib.GameData
  - Black Mage was being considered a crafting class in ClassJobCategoryRow
  - RenderTables no longer request the items every frame

### Added
- AllaganLib.Interface
  - IntegerFormField can now have a min and max value
  - MultipleChoiceFormField can now have it's results hidden
  - Columns can now be dynamically hidden
  - RenderTables can now have footers that are tables that automatically align themselves with the main table
  - RenderTables now get their item's via an async call
  - RenderTables can have a clipper enabled/disabled

## [1.2.11] - 2025-05-17

### Added
- AllaganLib.GameData
  - IsCollectable added to ItemRow

## [1.2.10] - 2025-05-06

### Added

- AllaganLib.GameData
  - Triple Triad sheets added
  - Triple Triad sources
  - Battle/Company/Craft/Gathering leve sources
  - NPC Shop cache now includes triple triad lookups

### Changes

- AllaganLib.GameData
  - Extra sources contain links to their items/cost items

## [1.2.9] - 2025-05-05

### Changes

- AllaganLib.Universalis
  - User-Agent is now sent with requests for both Universalis clients.

## [1.2.8] - 2025-05-01

### Changes

- AllaganLib.GameSheets
  - Updated LuminaSupplemental to 2.3.2
  - Added quest source/uses
  - Added map ids to duty sources

- AllaganLib.Universalis
  - Added back-off functionality to websocket service

## [1.2.7] - 2025-04-14

### Changes

- AllaganLib.GameSheets
  - Updated LuminaSupplemental to 2.3.1

- AllaganLib.Interface
  - DateTime columns will now order correctly

- All packages
  - Update nugets 

## [1.2.6] - 2025-04-14

### Changes

- AllaganLib.GameSheets
  - Special shop costs were not being collated properly

## [1.2.5] - 2025-04-09

### Changes

- AllaganLib.Universalis
    - Universalis dates were not being converted into local time

## [1.2.4] - 2025-04-08

### Changes

- AllaganLib.Interface
    - EnumFormField would not correctly display and would always think a value was set even if it was set to the default


## [1.2.3] - 2025-04-06

### Changes

- AllaganLib.Universalis
    - Add small delay to UniversalisWebSocket polling 

## [1.2.2] - 2025-04-05

### Changes

- AllaganLib.Interface
  - Fix ImGui asserts

- AllaganLib.Universalis
  - Stop item ID 0 from being added
  - Improvements to the UniversalisWebSocket service, now handles disconnects better, runs send/recieve in their own threads and can be enabled/disabled


## [1.2.1] - 2025-04-01

### Changes

- AllaganLib.GameData
    - Use TomestonesItem sheet to determine tomestone costs instead of hardcoded dictionary

## [1.2.0] - 2025-03-26

### Changes

- Initial support for API12/Patch 7.2

## [1.1.24] - 2025-03-16

### Changes

- AllaganLib.GameData
  - Fix certain shops having the wrong currency costs

## [1.1.23] - 2025-02-24

### Changes

- AllaganLib.GameData
  - Fix fishing spot map marker location
  - Fix some currency related issues for special shops

## [1.1.22] - 2025-02-12

### Changes

- AllaganLib.GameData
  - LuminaSupplemental.Excel updated to 2.2.3

## [1.1.21] - 2025-02-11

### Changes

- AllaganLib.GameData
  - LuminaSupplemental.Excel updated to 2.2.2

## [1.1.20] - 2025-02-11

### Changes

- AllaganLib.GameData
  - LuminaSupplemental.Excel updated to 2.2.1


## [1.1.19] - 2025-02-02

### Changes

- AllaganLib.GameData
  - LuminaSupplemental.Excel updated to 2.2.0


## [1.1.18] - 2025-02-01

### Changes

- AllaganLib.GameData
  - LuminaSupplemental.Excel updated to 2.1.9

## [1.1.17] - 2025-01-14

### Changes

- AllaganLib.GameData
  - LuminaSupplemental.Excel updated to 2.1.8
  - Fixed a bug with an empty item set being shown for glamour ready items
  - Stopped certain fishing sources from being generated that have no map
  - Added gathering points that were missing
  - GC shop sources now have the correct seal cost
  - Adjusted Glamour chest size
  - Gardening crossbreeds added as a source/use
  - Added spectacles as valid items that can be acquired

## [1.1.16] - 2025-01-06

### Changes

- AllaganLib.GameData
  - Added the gathering points required for dark matter clusters to be listed
  - Added company craft prototypes as uses on related items


## [1.1.15] - 2024-12-19

### Changes

- AllaganLib.GameData
  - Added the gathering points required for dark matter clusters to be listed
  - Added company craft prototypes as uses on related items


## [1.1.14] - 2024-12-17

### Changes

- AllaganLib.GameData
  - Bump LuminaSupplmental

## [1.1.13] - 2024-12-17

### Changes

- AllaganLib.GameData
  - Glamour Ready Items sources were split into the merged item and set items
  - Improvements to how fishing spots are determined
  - Improvements to how spearfishing spots are determined
  - Parse inclusion shop items
  - Subrow sheets can be requested directly via DI
  - Added Marker X/Y for FishingSpots, GatheringPoints, SpearfishingNotebook
  - GilShops will defer to the first NPCs name if they have a blank name

## [1.1.12] - 2024-12-11

### Changes

- AllaganLib.GameData
  - Class jobs in ClassJobCategory were not calculated properly

- AllaganLib.Interface
  - All fields now have a AutoSave property
  - FormFields DrawInput now return a boolean indicating if the value changed like imgui does
  - The MultiChoice form field now renders a preview and shows which items are selected
  - Added FloatRangeFormField and UintRangeFormFIeld

## [1.1.11] - 2024-12-04

### Changes

- AllaganLib.GameData
  - Added exterior house items and housing fixtures as uses
  - Fixed some minor bugs with gathering node classification
  - Fixed an issue with german characters

## [1.1.10] - 2024-11-28

### Changes

- AllaganLib.GameData
  - CustomTalk is now parsed when calculating NPC shops

## [1.1.9] - 2024-11-27

### Changes

- Build with correct dalamud-distrib

## [1.1.8] - 2024-11-27 

### Changes

- AllaganLib.GameData
  - Bump LuminaSupplemental
  - A large number of sources/uses were split into their own classes

### Fixes

- AllaganLib.GameData
  - Glamour Ready sets will calculate correctly
  - The correct amount of chocobo(buddy) item uses are now calculated
  - Spearfishing sources now list the correct maps
  - Glamour chest bumped to 8000(internally it has this?)

### Added

- AllaganLib.GameData
  - Added Expert Delivery Source(use)
  - Cash shop sources now have the price in USD

## [1.1.7] - 2024-11-21 

### Updated

- Updated LuminaSupplemental

### Added

- AllaganLib.GameData
  - Add new uses ItemStainSource and ItemGlamourReadySource


## [1.1.6] - 2024-11-18

### Fixed

- Use dalamud serilog
- Bump LuminaSupplemental.Excel
- SheetManager now accepts a ILogger in its options

## [1.1.5] - 2024-11-18

### Fixed

- AllaganLib.GameData
  - Added shared model methods on ItemRow and the classes required to determine the primary model string
  - Fixed a bug in the "IsItemEquppableBy" method in ClassJobCategorySheet

## [1.1.4] - 2024-11-18

### Fixed

- AllaganLib.GameData
  - Stopped SheetManager from always loading sheets in english
  - Stopped certain gathering points from showing up that had bad data
  - Added item uses: Chocobo Item, Indoor Furnishing
  - Added item sources: Achievement

## [1.1.3] - 2024-11-17

### Fixed

- AllaganLib.GameData
  - Remove dependency on DotBenchmark

## [1.1.2] - 2024-11-17

### Fixed

- AllaganLib.GameData
  - Updates to the sources/uses
  - Daily supply item now has a link to the reward row
  - Fix namespaces/formatting in AllaganLib.GameSheets

## [1.1.1] - 2024-11-16

### Added

- AllaganLib.GameData
  - Add AddonSheet
  - General improvements to the ItemInfoCache
  - Filter out -1 ingredients in the RecipeSheet
  - Improvements to uses

## [1.1.0] - 2024-11-13

### Added

- AllaganLib.GameData created, this provides a way of caching extra data on top of lumina. It allows for rows and sheets to be cached while still gaining the benefits of the improvements made in Lumina 5
- Initial support for API11

## [1.0.10] - 2024-09-25

### Fixed

- Universalis requests will pull all data and will also provide the last updated date.

## [1.0.9] - 2024-09-10

### Fixed

- Columns will now respect their ColumnFlags

### Added

- IconColumn added

## [1.0.8] - 2024-09-09

### Fixed

- Allow all columns to have their draw overridden

## [1.0.7] - 2024-09-08

### Fixed

- Fix column sorting on tables

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

