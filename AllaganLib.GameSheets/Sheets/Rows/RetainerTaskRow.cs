using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class RetainerTaskRow : ExtendedRow<RetainerTask, RetainerTaskRow, RetainerTaskSheet>
{
    private ClassJobCategoryRow? classJobCategoryRow;
    private RetainerTaskNormalRow? retainerTaskNormalRow;
    private RetainerTaskRandomRow? retainerTaskRandomRow;

    public ClassJobCategoryRow? ClassJobCategoryRow => this.Base.ClassJobCategory.IsValid ? this.classJobCategoryRow ??=
        this.Sheet.GetClassJobCategorySheet().GetRow(this.Base.ClassJobCategory.RowId) : null;

    public RetainerTaskNormalRow? RetainerTaskNormalRow => this.Base.Task.Is<RetainerTaskNormal>() ? this.retainerTaskNormalRow ??=
        this.Sheet.GetRetainerTaskNormalSheet().GetRow(this.Base.Task.RowId) : null;

    public RetainerTaskRandomRow? RetainerTaskRandomRow => this.Base.Task.Is<RetainerTaskRandom>() ? this.retainerTaskRandomRow ??=
        this.Sheet.GetRetainerTaskRandomSheet().GetRow(this.Base.Task.RowId) : null;

    public bool IsGatheringVenture => this.ClassJobCategoryRow?.IsGathering ?? false;

    public bool IsFishingVenture => this.ClassJobCategoryRow?.Base.FSH ?? false;

    public bool IsMiningVenture => this.ClassJobCategoryRow?.Base.MIN ?? false;

    public bool IsBotanistVenture => this.ClassJobCategoryRow?.Base.BTN ?? false;

    public bool IsCombatVenture => this.ClassJobCategoryRow?.IsCombat ?? false;

    public uint Quantity
    {
        get
        {
            if (this.RetainerTaskNormalRow != null)
            {
                return this.RetainerTaskNormalRow.Base.Quantity.LastOrDefault();
            }

            if (this.RetainerTaskRandomRow != null)
            {
                return 1;
            }

            return 0;
        }
    }

    public string Quantities
    {
        get
        {
            if (this.RetainerTaskNormalRow != null)
            {
                return this.RetainerTaskNormalRow.Quantities;
            }

            if (this.RetainerTaskRandomRow != null)
            {
                return "1";
            }

            return "0";
        }
    }

    public RetainerTaskType RetainerTaskType
    {
        get
        {
            if (this.Base.IsRandom)
            {
                if (this.IsFishingVenture)
                {
                    return RetainerTaskType.WatersideExploration;
                }
                else if (this.IsMiningVenture)
                {
                    return RetainerTaskType.HighlandExploration;
                }
                else if (this.IsBotanistVenture)
                {
                    return RetainerTaskType.WoodlandExploration;
                }
                else if (this.IsCombatVenture)
                {
                    return RetainerTaskType.FieldExploration;
                }
                else
                {
                    return RetainerTaskType.QuickExploration;
                }
            }
            else if (this.RetainerTaskNormalRow != null)
            {
                if (this.IsFishingVenture)
                {
                    return RetainerTaskType.Fishing;
                }
                else if (this.IsMiningVenture)
                {
                    return RetainerTaskType.Mining;
                }
                else if (this.IsBotanistVenture)
                {
                    return RetainerTaskType.Botanist;
                }
                else if (this.IsCombatVenture)
                {
                    return RetainerTaskType.Hunting;
                }
            }

            return RetainerTaskType.Unknown;
        }
    }

    private List<ItemRow>? drops;

    public List<ItemRow> Drops
    {
        get
        {
            if (this.drops != null)
            {
                return this.drops;
            }

            var dropsList = new List<ItemRow>();
            if (this.RetainerTaskRandomRow != null)
            {
                var ventureItems = this.Sheet.GetRetainerVentureItemsByTaskRandomId(this.RetainerTaskRandomRow.RowId);
                dropsList = ventureItems.Select(c => c.Item.IsValid ? this.Sheet.GetItemSheet().GetRow(c.Item.RowId) : null).Where(c => c != null).Select(c => c!).ToList();
            }
            else if (this.RetainerTaskNormalRow != null)
            {
                if (this.RetainerTaskNormalRow.Base.Item.IsValid)
                {
                    var itemRow = this.Sheet.GetItemSheet().GetRowOrDefault(this.RetainerTaskNormalRow.Base.Item.RowId);
                    if (itemRow != null)
                    {
                        dropsList.Add(itemRow);
                    }
                }
            }

            this.drops = dropsList;

            return this.drops;
        }
    }

    private string? formattedName;

    public string FormattedName
    {
        get
        {
            if (this.formattedName == null)
            {
                if (this.RetainerTaskNormalRow != null)
                {
                    this.formattedName = this.RetainerTaskNormalRow.TaskName;
                }
                else if (this.RetainerTaskRandomRow != null)
                {
                    this.formattedName = this.RetainerTaskRandomRow.FormattedName;
                }
                else
                {
                    this.formattedName = "Unknown Task";
                }
            }

            return this.formattedName;
        }
    }

    public string ExperienceString
    {
        get
        {
            if (this.Base.Experience > 0)
            {
                return this.Base.Experience.ToString();
            }

            return "N/A";
        }
    }

    public string DurationString
    {
        get
        {
            var time = TimeSpan.FromMinutes(this.Base.MaxTimemin);
            return $"{(int)time.TotalHours}h";
        }
    }
}