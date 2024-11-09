// <copyright file="NotoriousMonsterSheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class NotoriousMonsterSheet : ExtendedSheet<NotoriousMonster, NotoriousMonsterRow, NotoriousMonsterSheet>, IExtendedSheet
{
    public NotoriousMonsterSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}

public class NotoriousMonsterRow : ExtendedRow<NotoriousMonster, NotoriousMonsterRow, NotoriousMonsterSheet>
{
    public string RankFormatted()
    {
        return this.Base.Rank switch
        {
            1 => "B",
            2 => "A",
            3 => "S",
            _ => "N/A",
        };
    }
}