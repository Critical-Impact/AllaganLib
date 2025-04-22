# AllaganLib

AllaganLib.Data - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Data.svg)](https://www.nuget.org/packages/AllaganLib.Data)

AllaganLib.Shared - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Shared.svg)](https://www.nuget.org/packages/AllaganLib.Shared)

AllaganLib.GameSheets - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.GameSheets.svg)](https://www.nuget.org/packages/AllaganLib.GameSheets)

AllaganLib.Interface - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Interface.svg)](https://www.nuget.org/packages/AllaganLib.Interface)

AllaganLib.Universalis - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Universalis.svg)](https://www.nuget.org/packages/AllaganLib.Universalis)

---

AllaganLib is a collection of nuget packages for use within the Dalamud framework.

#### AllaganLib.Data
- CSV loading

#### AllaganLib.Shared
- Shared library utilized by the other nuget packages
- Time parsing for FFXIV time's system

#### AllaganLib.GameData
- Wrapper for Lumina, allowing for sheets and rows to be defined as actual classes
- Implements a variety of sheets already
- NpcLevelCache for locating where NPCs are within the game
- NpcShopCache for locating which NPCs relate to which shops
- ItemInfoCache for determining how items are sourced/used
- See SheetManager for more information

#### AllaganLib.Interface
- Class based system for form fields & grids
- Setup/configuration wizard
- ImGui widgets

#### AllaganLib.Universalis
- UnversalisApiService for direct requests to unversalis pricing data
- UniversalisWebsocketService for streaming access to universalis allowing you to subscribe to it's websocket and listen for events
