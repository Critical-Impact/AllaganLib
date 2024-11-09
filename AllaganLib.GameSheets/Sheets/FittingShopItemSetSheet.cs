// <copyright file="FittingShopItemSetSheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FittingShopItemSetSheet : ExtendedSheet<FittingShopItemSet, FittingShopItemSetRow, FittingShopItemSetSheet>, IExtendedSheet
{
    public FittingShopItemSetSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}

public class FittingShopItemSetRow : ExtendedRow<FittingShopItemSet, FittingShopItemSetRow, FittingShopItemSetSheet>
{

}