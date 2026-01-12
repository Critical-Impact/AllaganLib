using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class BNpcBaseRow : ExtendedRow<BNpcBase, BNpcBaseRow, BNpcBaseSheet>
{
    private NpcType? _npcType;
    private BNpcNameRow _nameRow;
    private Dictionary<EquipSlot, List<ItemRow>>? relatedItems;

    public NpcType NpcType
    {
        get
        {
            if (this._npcType == null)
            {
                var modelChara = this.Base.ModelChara.Value;
                switch (modelChara.Type)
                {
                    case 0:
                        this._npcType = NpcType.Misc;
                        break;
                    case 1:
                        this._npcType = NpcType.Humanoid;
                        break;
                    case 2:
                        this._npcType = NpcType.Monster;
                        break;
                    case 3:
                        this._npcType = NpcType.Monster;
                        break;
                    case 4:
                        this._npcType = NpcType.Nest;
                        break;
                }

                this._npcType ??= NpcType.Unknown;
            }

            return this._npcType.Value;
        }
    }

    public List<ItemRow> GetRelatedItems()
    {
        return this.GetRelatedItems(Enum.GetValues<EquipSlot>());
    }

    public List<ItemRow> GetRelatedItems(params EquipSlot[] slots)
    {
        var items = new List<ItemRow>();
        foreach (var slotType in slots)
        {
            items.AddRange(this.GetRelatedItems(slotType));
        }

        return items;
    }

    public List<ItemRow> GetRelatedItems(EquipSlot slot)
    {
        this.relatedItems ??= new Dictionary<EquipSlot, List<ItemRow>>();
        if (!this.relatedItems.TryGetValue(slot, out var items))
        {
            uint modelKey;
            bool includeVariants = true;
            var npcEquip = this.Base.NpcEquip.RowId == 0 ? null : this.Base.NpcEquip.ValueNullable;
            if (npcEquip == null)
            {
                this.relatedItems[slot] = [];
                return this.relatedItems[slot];
            }
            switch (slot)
            {
                case EquipSlot.MainHand:
                    modelKey = (uint)npcEquip.Value.ModelMainHand;
                    includeVariants = false;
                    break;
                case EquipSlot.OffHand:
                    modelKey = (uint)npcEquip.Value.ModelOffHand;
                    includeVariants = false;
                    break;
                case EquipSlot.Head:
                    modelKey = npcEquip.Value.ModelHead;
                    break;
                case EquipSlot.Body:
                    modelKey = npcEquip.Value.ModelBody;
                    break;
                case EquipSlot.Gloves:
                    modelKey = npcEquip.Value.ModelHands;
                    break;
                case EquipSlot.Legs:
                    modelKey = npcEquip.Value.ModelLegs;
                    break;
                case EquipSlot.Feet:
                    modelKey = npcEquip.Value.ModelFeet;
                    break;
                case EquipSlot.Ears:
                    modelKey = npcEquip.Value.ModelEars;
                    break;
                case EquipSlot.Neck:
                    modelKey = npcEquip.Value.ModelNeck;
                    break;
                case EquipSlot.Wrists:
                    modelKey = npcEquip.Value.ModelWrists;
                    break;
                case EquipSlot.FingerR:
                    modelKey = npcEquip.Value.ModelRightRing;
                    break;
                case EquipSlot.FingerL:
                    modelKey = npcEquip.Value.ModelLeftRing;
                    break;
                default:
                    this.relatedItems[slot] = [];
                    return this.relatedItems[slot];
            }

            var modelBase = (short)modelKey;
            var modelVariant = (short)(modelKey >> 16);
            if (modelBase == 0)
            {
                this.relatedItems[slot] = [];
            }
            else
            {
                this.relatedItems[slot] = this.Sheet.GetItemSheet()
                        .Where(c => c.ModelBase == modelBase && c.ModelVariant == modelVariant && c.EquipSlotCategory != null && c.EquipSlotCategory.PossibleSlots.Contains(slot)).ToList();
                if (includeVariants && !this.relatedItems[slot].Any())
                {
                    this.relatedItems[slot] = this.Sheet.GetItemSheet()
                        .Where(c => c.ModelBase == modelBase && c.EquipSlotCategory != null && c.EquipSlotCategory.PossibleSlots.Contains(slot)).ToList();
                }
            }

            return this.relatedItems[slot];
        }
        return items;
    }
}