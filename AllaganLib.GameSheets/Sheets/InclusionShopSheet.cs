using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class InclusionShopSheet : ExtendedSheet<InclusionShop, InclusionShopRow, InclusionShopSheet>, IExtendedSheet
{
    public Dictionary<uint, HashSet<InclusionShopRow>>? inclusionShopsByCategoryId = null;

    public InclusionShopSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public HashSet<InclusionShopRow>? GetInclusionShopsByCategoryId(uint categoryId)
    {
        if (this.inclusionShopsByCategoryId == null)
        {
            this.inclusionShopsByCategoryId = new Dictionary<uint, HashSet<InclusionShopRow>>();
            foreach (var shopRow in this)
            {
                var categories = shopRow.Base.Category;
                foreach (var category in categories)
                {
                    if (category.RowId == 0)
                    {
                        continue;
                    }
                    this.inclusionShopsByCategoryId.TryAdd(category.RowId, []);
                    this.inclusionShopsByCategoryId[category.RowId].Add(shopRow);
                }
            }
        }

        return this.inclusionShopsByCategoryId.GetValueOrDefault(categoryId);
    }

    public override void CalculateLookups()
    {

    }
}