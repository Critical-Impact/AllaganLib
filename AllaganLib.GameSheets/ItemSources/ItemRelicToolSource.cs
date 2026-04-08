using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemMastercraftToolSource : ItemRelicToolSource
{
    public ItemMastercraftToolSource(ItemRow item, RelicTool relicTool, List<RelicTool> forms, List<ItemRow> relatedItems)
        : base(item, relicTool, forms, relatedItems, ItemInfoType.MastercraftTool)
    {
    }
}

public class ItemSkysteelToolSource : ItemRelicToolSource
{
    public ItemSkysteelToolSource(ItemRow item, RelicTool relicTool, List<RelicTool> forms, List<ItemRow> relatedItems)
        : base(item, relicTool, forms, relatedItems, ItemInfoType.SkysteelTool)
    {
    }
}

public class ItemResplendentToolSource : ItemRelicToolSource
{
    public ItemResplendentToolSource(ItemRow item, RelicTool relicTool, List<RelicTool> forms, List<ItemRow> relatedItems)
        : base(item, relicTool, forms, relatedItems, ItemInfoType.ResplendentTool)
    {
    }
}

public class ItemSplendorousToolSource : ItemRelicToolSource
{
    public ItemSplendorousToolSource(ItemRow item, RelicTool relicTool, List<RelicTool> forms, List<ItemRow> relatedItems)
        : base(item, relicTool, forms, relatedItems, ItemInfoType.SplendorousTool)
    {
    }
}

public class ItemCosmicToolSource : ItemRelicToolSource
{
    public ItemCosmicToolSource(ItemRow item, RelicTool relicTool, List<RelicTool> forms, List<ItemRow> relatedItems)
        : base(item, relicTool, forms, relatedItems, ItemInfoType.CosmicTool)
    {
    }
}

public abstract class ItemRelicToolSource : ItemSource
{
    public RelicTool RelicTool { get; }

    public List<RelicTool> Forms { get; }

    public List<ItemRow> Items;

    public ItemRelicToolSource(ItemRow item, RelicTool relicTool, List<RelicTool> forms, List<ItemRow> relatedItems, ItemInfoType itemInfoType)
        : base(itemInfoType)
    {
        this.RelicTool = relicTool;
        this.Forms = forms;
        this.Item = item;
        this.Items = relatedItems;
    }

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return this.Items.Select(c => ItemInfo.Create(c)).ToList();
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.RelatedTo;
}