using System.Collections.Generic;

namespace AllaganLib.GameSheets.Caches;

public abstract class GenericItemSource
{
    public ItemInfoType Type { get; protected set; }

    public abstract uint Quantity { get; }

    public abstract HashSet<uint>? MapIds { get; }
}