// <copyright file="SatisfactionSupplySheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SatisfactionSupplySheet : ExtendedSubrowSheet<SatisfactionSupply, SatisfactionSupplyRow, SatisfactionSupplySheet>, IExtendedSheet
{
    public SatisfactionSupplySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer)
        : base(gameData, sheetManager, sheetIndexer)
    {
    }

    public override void CalculateLookups()
    {
    }
}
public class SatisfactionSupplyRow : ExtendedSubrow<SatisfactionSupply, SatisfactionSupplyRow, SatisfactionSupplySheet>
{

}