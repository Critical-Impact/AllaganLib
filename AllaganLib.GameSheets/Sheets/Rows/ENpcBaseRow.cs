using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ENpcBaseRow : ExtendedRow<ENpcBase, ENpcBaseRow, ENpcBaseSheet>, INamed
{
    private Dictionary<EquipSlot, List<ItemRow>>? relatedItems;
    private List<ILocation>? locations;
    private RowRef<ENpcResident>? eNpcResident;
    private ENpcResidentRow? residentRow;

    public bool IsVendor => this.Sheet.IsVendor(this.RowId);

    public bool IsHouseVendor => this.Sheet.IsHouseVendor(this.RowId);

    public bool IsHouseVendorChild => this.Sheet.IsHouseVendorChild(this.RowId);

    public bool IsCalamitySalvager => HardcodedItems.CalamitySalvagers.Contains(this.RowId);

    public RowRef<ENpcResident> Resident
    {
        get { return this.eNpcResident ??= new RowRef<ENpcResident>(this.Sheet.GameData.Excel, this.RowId); }
    }


    public ENpcResidentRow ENpcResidentRow
    {
        get { return this.residentRow ??= this.Sheet.GetENpcResidentSheet().GetRow(this.RowId); }
    }

    public IEnumerable<ILocation> Locations
    {
        get { return this.locations ??= this.Sheet.GetLocations(this.RowId)?.Cast<ILocation>().ToList() ?? []; }
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
            switch (slot)
            {
                case EquipSlot.MainHand:
                    modelKey = (uint)(npcEquip?.ModelMainHand ?? this.Base.ModelMainHand);
                    includeVariants = false;
                    break;
                case EquipSlot.OffHand:
                    modelKey = (uint)(npcEquip?.ModelOffHand ?? this.Base.ModelOffHand);
                    includeVariants = false;
                    break;
                case EquipSlot.Head:
                    modelKey = npcEquip?.ModelHead ?? this.Base.ModelHead;
                    break;
                case EquipSlot.Body:
                    modelKey = npcEquip?.ModelBody ?? this.Base.ModelBody;
                    break;
                case EquipSlot.Gloves:
                    modelKey = npcEquip?.ModelHands ?? this.Base.ModelHands;
                    break;
                case EquipSlot.Legs:
                    modelKey = npcEquip?.ModelLegs ?? this.Base.ModelLegs;
                    break;
                case EquipSlot.Feet:
                    modelKey = npcEquip?.ModelFeet ?? this.Base.ModelFeet;
                    break;
                case EquipSlot.Ears:
                    modelKey = npcEquip?.ModelEars ?? this.Base.ModelEars;
                    break;
                case EquipSlot.Neck:
                    modelKey = npcEquip?.ModelNeck ?? this.Base.ModelNeck;
                    break;
                case EquipSlot.Wrists:
                    modelKey = npcEquip?.ModelWrists ?? this.Base.ModelWrists;
                    break;
                case EquipSlot.FingerR:
                    modelKey = npcEquip?.ModelRightRing ?? this.Base.ModelRightRing;
                    break;
                case EquipSlot.FingerL:
                    modelKey = npcEquip?.ModelLeftRing ?? this.Base.ModelLeftRing;
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

    public string Name => this.ENpcResidentRow.Name;
}