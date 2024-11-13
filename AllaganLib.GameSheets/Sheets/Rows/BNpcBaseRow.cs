using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class BNpcBaseRow : ExtendedRow<BNpcBase, BNpcBaseRow, BNpcBaseSheet>
{
    private NpcType? _npcType;
    private BNpcNameRow _nameRow;

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
}