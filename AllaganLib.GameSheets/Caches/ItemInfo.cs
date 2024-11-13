using System.Collections.Generic;

namespace AllaganLib.GameSheets.Caches;

public abstract class ItemInfo
{
    public ItemInfoType Type { get; protected set; }

    public abstract uint Quantity { get; }

    public abstract HashSet<uint>? MapIds { get; }
}