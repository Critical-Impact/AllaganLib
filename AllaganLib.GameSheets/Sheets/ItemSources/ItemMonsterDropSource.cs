using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemMonsterDropSource : ItemSource
{
    private readonly BNpcNameRow bNpcName;

    public ItemMonsterDropSource(ItemRow item, BNpcNameRow bNpcName) : base(ItemInfoType.Monster)
    {
        this.bNpcName = bNpcName;
        this.Item = item;
    }

    public override uint Quantity => 1;

    public override string Name => this.BNpcName.Base.Singular.ExtractText();

    public override uint Icon => 60041;

    public BNpcNameRow BNpcName => this.bNpcName;

    public override HashSet<uint>? MapIds => this.bNpcName.MapIds;
}