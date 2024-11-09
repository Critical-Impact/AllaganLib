// <copyright file="AquariumFishSheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class AquariumFishSheet : ExtendedSheet<AquariumFish, AquariumFishRow, AquariumFishSheet>, IExtendedSheet
{
    public AquariumFishSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {

    }
}

public class AquariumFishRow : ExtendedRow<AquariumFish, AquariumFishRow, AquariumFishSheet>
{

}