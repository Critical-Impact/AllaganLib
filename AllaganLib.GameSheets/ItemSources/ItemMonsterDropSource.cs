using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemMonsterDropSource : ItemSource
{
    public MobDrop MobDrop { get; }

    private readonly BNpcNameRow bNpcName;

    public ItemMonsterDropSource(ItemRow item, BNpcNameRow bNpcName, MobDrop mobDrop) : base(ItemInfoType.Monster)
    {
        this.MobDrop = mobDrop;
        this.bNpcName = bNpcName;
        this.Item = item;
    }

    public override uint Quantity => 1;

    public BNpcNameRow BNpcName => this.bNpcName;

    public override HashSet<uint>? MapIds => this.bNpcName.MapIds;
}