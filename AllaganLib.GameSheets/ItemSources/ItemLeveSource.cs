using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemLeveSource : ItemSource
{
    public RowRef<Leve> Leve { get; }
    
    public RowRef<LeveRewardItem> LeveRewardItem { get; }
    
    public int RewardItemIndex { get; }
    
    public RowRef<LeveRewardItemGroup> LeveRewardItemGroup { get; }
    
    public RowRef<ParamGrow> ParamGrow;
    
    public virtual int ExpReward => (int)(this.ParamGrow.Value.ScaledQuestXP * (decimal)this.ParamGrow.Value.QuestExpModifier * (decimal)this.Leve.Value.ExpFactor) + 1;
    
    public int RewardItemGroupIndex { get; }
    
    public override uint Quantity => this.LeveRewardItemGroup.Value.Count[this.RewardItemGroupIndex];
    
    public bool IsHq => this.LeveRewardItemGroup.Value.IsHQ[this.RewardItemGroupIndex];
    
    public ItemLeveSource(RowRef<Leve> leve, RowRef<LeveRewardItem> leveRewardItem, int rewardItemIndex, RowRef<LeveRewardItemGroup> leveRewardItemGroup, int rewardItemGroupIndex, ItemRow item, ItemInfoType type)
        : base(type)
    {
        this.Leve = leve;
        this.LeveRewardItem = leveRewardItem;
        this.RewardItemIndex = rewardItemIndex;
        this.LeveRewardItemGroup = leveRewardItemGroup;
        this.RewardItemGroupIndex = rewardItemGroupIndex;
        this.Item = item;
        this.ParamGrow = new RowRef<ParamGrow>(item.Sheet.GameData.Excel, this.Leve.Value.ClassJobLevel);
    }
}