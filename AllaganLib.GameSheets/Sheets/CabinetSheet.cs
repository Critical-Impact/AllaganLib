// <copyright file="CabinetSheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CabinetSheet : ExtendedSheet<Cabinet, CabinetRow, CabinetSheet>, IExtendedSheet
{
    private int? cabinetSize;
    private CabinetCategorySheet? cabinetCategorySheet;

    public CabinetSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public CabinetCategorySheet CabinetCategorySheet => this.cabinetCategorySheet ??= this.SheetManager.GetSheet<CabinetCategorySheet>();

    public int CabinetSize => this.cabinetSize ??= this.Count;
}

public class CabinetRow : ExtendedRow<Cabinet, CabinetRow, CabinetSheet>
{
    private CabinetCategoryRow? cabinetCategory;

    public CabinetCategoryRow? CabinetCategory => this.cabinetCategory ??= this.Sheet.CabinetCategorySheet.GetRowOrDefault(this.Base.Category.RowId);
}