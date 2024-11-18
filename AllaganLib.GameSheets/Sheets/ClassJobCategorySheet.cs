using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class ClassJobCategorySheet : ExtendedSheet<ClassJobCategory, ClassJobCategoryRow, ClassJobCategorySheet>, IExtendedSheet
{
    private ClassJobSheet? classJobSheet;
    private Dictionary<uint, HashSet<uint>> classJobCategoryLookup;
    private bool classJobCategoryLookupCalculated;

    public ClassJobCategorySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public ClassJobSheet GetClassJobSheet()
    {
        return this.classJobSheet ??= this.SheetManager.GetSheet<ClassJobSheet>();
    }

    public bool IsItemEquippableBy(uint classJobCategory, uint classJobId)
    {
        this.CalculateClassJobCategoryLookup();
        return this.classJobCategoryLookup.TryGetValue(classJobCategory, out var value) && value.Contains(classJobId);
    }

    private void CalculateClassJobCategoryLookup()
    {
        if (!this.classJobCategoryLookupCalculated)
        {
            var classJobMap = new Dictionary<string, uint>();
            foreach (var classJob in this.SheetManager.GetSheet<ClassJobSheet>())
            {
                if (!classJobMap.ContainsKey(classJob.Base.Abbreviation.ExtractText()))
                {
                    classJobMap[classJob.Base.Abbreviation.ExtractText()] = classJob.RowId;
                }
            }

            this.classJobCategoryLookupCalculated = true;
            var classJobCategoryMap = new Dictionary<uint, HashSet<uint>>();
            var propertyInfos = typeof(ClassJobCategory).GetProperties().Where(c => c.PropertyType == typeof(bool))
                .ToList();

            foreach (var classJobCategory in this)
            {
                if (classJobCategory.RowId == 0) continue;

                //Dont hate me, there's now probably a better way to do this now
                var map = new HashSet<uint>();
                foreach (var prop in propertyInfos)
                {
                    var parsed = prop.GetValue(classJobCategory.Base, null);
                    if (parsed is bool b && (bool?)b == true)
                    {
                        if (classJobMap.TryGetValue(prop.Name, out var classJobRowId))
                        {
                            map.Add(classJobRowId);
                        }
                    }
                }

                classJobCategoryMap[classJobCategory.RowId] = map;
            }

            this.classJobCategoryLookup = classJobCategoryMap;
        }
    }
}