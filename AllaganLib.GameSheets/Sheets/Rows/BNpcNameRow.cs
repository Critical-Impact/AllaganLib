using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class BNpcNameRow : ExtendedRow<BNpcName, BNpcNameRow, BNpcNameSheet>
{
    private List<MobDrop>? mobDrops;
    private List<MobSpawnPosition>? mobSpawnPosition;
    private List<uint>? relatedBaseIds;
    private List<BNpcBaseRow>? relatedBases;
    private HashSet<uint>? mapIds;
    private HashSet<NpcType>? mobTypes;


    public string GarlandToolsId => this.RelatedBases.First().RowId + "0000000" + this.RowId;

    public List<BNpcBaseRow> RelatedBases
    {
        get
        {
            if (this.relatedBases == null)
            {
                var bNpcBaseSheet = this.Sheet.GetBNpcBaseSheet();
                this.relatedBases = this.RelatedBaseIds.Select(c => bNpcBaseSheet.GetRow(c)).ToList();
            }

            return this.relatedBases;
        }
    }

    public HashSet<NpcType> MobTypes
    {
        get
        {
            if (this.mobTypes == null)
            {
                this.mobTypes = this.RelatedBases.Select(lazyRow => lazyRow.NpcType).Distinct().ToHashSet();
            }

            return this.mobTypes;
        }
    }

    public List<uint> RelatedBaseIds => this.relatedBaseIds ??= this.Sheet.GetRelatedBasesByBNpcNameId(this.RowId);

    public List<MobDrop> MobDrops => this.mobDrops ??= this.Sheet.GetMobDropsByBNpcNameId(this.RowId);

    public HashSet<uint> MapIds => this.mapIds ??= this.MobSpawnPositions.Where(c => c.TerritoryType.IsValid)
        .Select(d => d.TerritoryType.Value.Map.RowId).Distinct().ToHashSet();

    public List<MobSpawnPosition> MobSpawnPositions => this.mobSpawnPosition ??= this.Sheet.GetMobSpawnPositionsByBNpcNameId(this.RowId);

    public NotoriousMonsterRow? NotoriousMonster => this.Sheet.GetNotoriousMonsterSheet().GetRowOrDefault(this.RowId);

}