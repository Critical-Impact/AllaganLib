using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCompanyLeveSource : ItemSource
{
    public RowRef<CompanyLeve> CompanyLeve { get; }

    public RowRef<Leve> Leve { get; }

    public uint SealsRewarded { get; }

    public RowRef<ParamGrow> ParamGrow;

    public virtual int ExpReward => (int)(this.ParamGrow.Value.ScaledQuestXP * (decimal)this.ParamGrow.Value.QuestExpModifier * (decimal)this.Leve.Value.ExpFactor);

    public ItemCompanyLeveSource(RowRef<CompanyLeve> companyLeve, RowRef<Leve> leve, ItemRow item)
        : base(ItemInfoType.CompanyLeve)
    {
        this.CompanyLeve = companyLeve;
        this.Leve = leve;
        this.Item = item;
        this.ParamGrow = new RowRef<ParamGrow>(item.Sheet.GameData.Excel, this.Leve.Value.ClassJobLevel);
        var leveSystemDefine = this.Item.Sheet.GameData.GetExcelSheet<LeveSystemDefine>()!.GetRow(23);
        decimal baseSeals = leveSystemDefine.Unknown1 * 0.01M;
        this.SealsRewarded = (uint)(leve.Value.ClassJobLevel * baseSeals);
    }

    public override uint Quantity => this.SealsRewarded;

    public override RelationshipType RelationshipType => RelationshipType.Rewards;
}