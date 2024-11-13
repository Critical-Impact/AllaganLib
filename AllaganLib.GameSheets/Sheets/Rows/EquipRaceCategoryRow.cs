using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class EquipRaceCategoryRow : ExtendedRow<EquipRaceCategory, EquipRaceCategoryRow, EquipRaceCategorySheet>
{
    public bool AllowsRace(CharacterRace race)
    {
        var raceId = (short)race;
        return raceId switch
        {
            0 => false,
            1 => this.Base.Hyur,
            2 => this.Base.Elezen,
            3 => this.Base.Lalafell,
            4 => this.Base.Miqote,
            5 => this.Base.Roegadyn,
            6 => this.Base.AuRa,
            7 => this.Base.Unknown0, // Hrothgar
            8 => this.Base.Unknown1, // Viera
            99 => true,
            _ => false,
        };
    }

    public bool AllowsRaceSex(CharacterRace race, CharacterSex sex)
    {
        var raceId = (short)race;
        return sex switch
        {
            CharacterSex.Both when this.Base.Male == false || this.Base.Female == false => false,
            CharacterSex.Either when this.Base.Male == false && this.Base.Female == false => false,
            CharacterSex.Female when this.Base.Female == false => false,
            CharacterSex.FemaleOnly when this.Base.Male || !this.Base.Female => false,
            CharacterSex.MaleOnly when this.Base.Female || !this.Base.Male => false,
            CharacterSex.Male when this.Base.Male == false => false,
            _ => raceId switch
            {
                0 => false,
                1 => this.Base.Hyur,
                2 => this.Base.Elezen,
                3 => this.Base.Lalafell,
                4 => this.Base.Miqote,
                5 => this.Base.Roegadyn,
                6 => this.Base.AuRa,
                7 => this.Base.Unknown0, // Hrothgar
                8 => this.Base.Unknown1, // Viera
                99 => true,
                _ => false,
            },
        };
    }


    public CharacterRace EquipRace
    {
        get
        {
            if (this.Base is { Hyur: true, Elezen: true, Lalafell: true, Miqote: true, Roegadyn: true, AuRa: true, Unknown0: true, Unknown1: true })
            {
                return CharacterRace.Any;
            }

            if (this.Base.Hyur)
            {
                return CharacterRace.Hyur;
            }

            if (this.Base.Elezen)
            {
                return CharacterRace.Elezen;
            }

            if (this.Base.Lalafell)
            {
                return CharacterRace.Lalafell;
            }

            if (this.Base.Miqote)
            {
                return CharacterRace.Miqote;
            }

            if (this.Base.Roegadyn)
            {
                return CharacterRace.Roegadyn;
            }

            if (this.Base.AuRa)
            {
                return CharacterRace.AuRa;
            }

            if (this.Base.Unknown0)
            {
                return CharacterRace.Hrothgar;
            }

            if (this.Base.Unknown1)
            {
                return CharacterRace.Viera;
            }

            return CharacterRace.None;
        }
    }
}