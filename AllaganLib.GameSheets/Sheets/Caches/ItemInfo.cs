using System.Collections.Generic;

namespace AllaganLib.GameSheets.Sheets.Caches;

public abstract class ItemInfo
{
    public ItemInfoType Type { get; protected set; }

    public abstract uint Quantity { get; }

    public abstract string Name { get; }

    public abstract uint Icon { get; }

    public abstract HashSet<uint>? MapIds { get; }
}