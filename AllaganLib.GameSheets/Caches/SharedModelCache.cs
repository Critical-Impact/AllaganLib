using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Caches;


public sealed class SharedModelCache : IReadOnlyList<SharedModelCache.SharedModelGroup>
{
    private readonly ItemSheet itemSheet;

    private readonly Lazy<IReadOnlyList<SharedModelGroup>> sharedModels;

    public SharedModelCache(ItemSheet itemSheet)
    {
        this.itemSheet = itemSheet;

        this.sharedModels = new Lazy<IReadOnlyList<SharedModelGroup>>(
            this.BuildCache,
            isThreadSafe: true);
    }

    public int Count => this.sharedModels.Value.Count;

    public SharedModelGroup this[int index] => this.sharedModels.Value[index];

    public IEnumerator<SharedModelGroup> GetEnumerator() => this.sharedModels.Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private IReadOnlyList<SharedModelGroup> BuildCache()
    {
        var groups = new Dictionary<string, List<ItemRow>>();

        foreach (var item in this.itemSheet)
        {
            var key = item.GetPrimaryModelKeyString();
            if (string.IsNullOrEmpty(key))
                continue;

            if (!groups.TryGetValue(key, out var list))
            {
                list = new List<ItemRow>();
                groups[key] = list;
            }

            list.Add(item);
        }

        // Only keep shared models (>1 item)
        return groups
            .Where(kvp => kvp.Value.Count > 1)
            .Select(kvp => new SharedModelGroup(kvp.Key, kvp.Value))
            .ToList();
    }

    public readonly record struct SharedModelGroup(
        string ModelKey,
        IReadOnlyList<ItemRow> Items);
}