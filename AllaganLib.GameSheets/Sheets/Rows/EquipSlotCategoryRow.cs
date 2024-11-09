using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class EquipSlotCategoryRow : ExtendedRow<EquipSlotCategory, EquipSlotCategoryRow, EquipSlotCategorySheet>
{
    private HashSet<EquipSlot>? possibleSlots = null!;
    private HashSet<EquipSlot>? blockedSlots = null!;

    public HashSet<EquipSlot> PossibleSlots
    {
        get
        {
            if (this.possibleSlots == null)
            {
                this.Build();
            }

            return this.possibleSlots!;
        }
    }

    public HashSet<EquipSlot> BlockedSlots
    {
        get
        {
            if (this.blockedSlots == null)
            {
                this.Build();
            }

            return this.blockedSlots!;
        }
    }


    public bool IsPossibleSlot(EquipSlot slot)
    {
        return this.PossibleSlots.Contains(slot);
    }

    public bool IsBlockedSlot(EquipSlot slot)
    {
        return this.BlockedSlots.Contains(slot);
    }

    public bool SimilarSlots(ItemRow item)
    {
        return this.PossibleSlots.Any(c => item.EquipSlotCategory?.PossibleSlots.Contains(c) ?? false);
    }

    private void Build()
    {
        var possible = new HashSet<EquipSlot>();
        var blocked = new HashSet<EquipSlot>();
        var equipSlots = new[]
        {
            EquipSlot.Body, EquipSlot.Ears, EquipSlot.Feet, EquipSlot.Gloves, EquipSlot.Head, EquipSlot.Legs,
            EquipSlot.Neck, EquipSlot.Waist, EquipSlot.Wrists, EquipSlot.FingerL, EquipSlot.FingerR, EquipSlot.MainHand,
            EquipSlot.OffHand, EquipSlot.SoulCrystal,
        };
        foreach (var equipSlot in equipSlots)
        {
            var val = 0;
            switch (equipSlot)
            {
                case EquipSlot.Body:
                    val = this.Base.Body;
                    break;
                case EquipSlot.Ears:
                    val = this.Base.Ears;
                    break;
                case EquipSlot.Feet:
                    val = this.Base.Feet;
                    break;
                case EquipSlot.Gloves:
                    val = this.Base.Gloves;
                    break;
                case EquipSlot.Head:
                    val = this.Base.Head;
                    break;
                case EquipSlot.Legs:
                    val = this.Base.Legs;
                    break;
                case EquipSlot.Neck:
                    val = this.Base.Neck;
                    break;
                case EquipSlot.Waist:
                    val = this.Base.Waist;
                    break;
                case EquipSlot.Wrists:
                    val = this.Base.Wrists;
                    break;
                case EquipSlot.FingerL:
                    val = this.Base.FingerL;
                    break;
                case EquipSlot.FingerR:
                    val = this.Base.FingerR;
                    break;
                case EquipSlot.MainHand:
                    val = this.Base.MainHand;
                    break;
                case EquipSlot.OffHand:
                    val = this.Base.OffHand;
                    break;
                case EquipSlot.SoulCrystal:
                    val = this.Base.SoulCrystal;
                    break;
            }

            if (val > 0 && !possible.Contains(equipSlot))
            {
                possible.Add(equipSlot);
            }
            else if (val < 0 && !blocked.Contains(equipSlot))
            {
                blocked.Add(equipSlot);
            }
        }

        this.possibleSlots = possible;
        this.blockedSlots = blocked;
    }
}