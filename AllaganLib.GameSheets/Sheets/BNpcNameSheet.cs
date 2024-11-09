using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class BNpcNameSheet : ExtendedSheet<BNpcName, BNpcNameRow, BNpcNameSheet>, IExtendedSheet
{
    private readonly List<MobSpawnPosition> mobSpawnPositions;
    private readonly List<MobDrop> mobDrops;

    private Dictionary<uint, List<MobSpawnPosition>> mobSpawnPositionsLookup;
    private Dictionary<uint, List<MobDrop>> mobDropsLookup;
    private Dictionary<uint, List<uint>> relatedBases;
    private BNpcBaseSheet? bNpcBaseSheet;
    private NotoriousMonsterSheet? notoriousMonsterSheet;

    public BNpcNameSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, List<MobSpawnPosition> mobSpawnPositions, List<MobDrop> mobDrops)
        : base(
        gameData,
        sheetManager,
        sheetIndexer,
        itemInfoCache)
    {
        this.mobSpawnPositions = mobSpawnPositions;
        this.mobDrops = mobDrops;

        this.mobSpawnPositionsLookup = new Dictionary<uint, List<MobSpawnPosition>>();
        this.mobDropsLookup = new Dictionary<uint, List<MobDrop>>();
        this.relatedBases = new Dictionary<uint, List<uint>>();
    }

    public BNpcBaseSheet GetBNpcBaseSheet()
    {
        return this.bNpcBaseSheet ??= this.SheetManager.GetSheet<BNpcBaseSheet>();
    }

    public NotoriousMonsterSheet GetNotoriousMonsterSheet()
    {
        return this.notoriousMonsterSheet ??= this.SheetManager.GetSheet<NotoriousMonsterSheet>();
    }

    public override void CalculateLookups()
    {
        this.mobSpawnPositionsLookup.Clear();
        this.mobDropsLookup.Clear();
        this.relatedBases.Clear();

        this.mobSpawnPositionsLookup = this.mobSpawnPositions.GroupBy(c => c.BNpcNameId).ToDictionary(c => c.Key, c => c.ToList());
        this.mobDropsLookup = this.mobDrops.GroupBy(c => c.BNpcNameId).ToDictionary(c => c.Key, c => c.ToList());

        this.relatedBases = this.mobSpawnPositions.GroupBy(c => c.BNpcNameId).ToDictionary(c => c.Key, c => c.Select(d => d.BNpcBaseId).Distinct().ToList());
    }

    public List<MobSpawnPosition> GetMobSpawnPositionsByBNpcNameId(uint bnpcNameId)
    {
        if (this.mobSpawnPositionsLookup.TryGetValue(bnpcNameId, out var spawnPositions))
        {
            return spawnPositions;
        }

        return new List<MobSpawnPosition>();
    }

    public List<MobDrop> GetMobDropsByBNpcNameId(uint bnpcNameId)
    {
        if (this.mobDropsLookup.TryGetValue(bnpcNameId, out var spawnPositions))
        {
            return spawnPositions;
        }

        return new List<MobDrop>();
    }

    public List<uint> GetRelatedBasesByBNpcNameId(uint bnpcNameId)
    {
        if (this.relatedBases.TryGetValue(bnpcNameId, out var bases))
        {
            return bases;
        }

        return new List<uint>();
    }
}