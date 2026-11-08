using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class NotoriousMonsterSheet : ExtendedSheet<NotoriousMonster, NotoriousMonsterRow, NotoriousMonsterSheet>, IExtendedSheet
{
    private Dictionary<uint, NotoriousMonsterRow>? notoriousMonstersByBNpcName;

    public NotoriousMonsterSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
        this.notoriousMonstersByBNpcName = null;
    }

    /// <summary>
    /// Gets the notorious monster for a BNpcName. NotoriousMonster rows have their own row IDs, so they cannot be looked up by the BNpcName ID directly.
    /// When more than one row points to the same BNpcName, the row with the highest rank is returned.
    /// </summary>
    /// <param name="bNpcNameId">The BNpcName row ID.</param>
    /// <returns>The notorious monster, or null if the BNpcName is not a notorious monster.</returns>
    public NotoriousMonsterRow? GetByBNpcNameId(uint bNpcNameId)
    {
        this.notoriousMonstersByBNpcName ??= this
            .Where(c => c.Base.BNpcName.RowId != 0)
            .GroupBy(c => c.Base.BNpcName.RowId)
            .ToDictionary(c => c.Key, c => c.OrderByDescending(d => d.Base.Rank).First());

        return this.notoriousMonstersByBNpcName.GetValueOrDefault(bNpcNameId);
    }
}
