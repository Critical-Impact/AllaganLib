using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Service;
using Dalamud.Plugin;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Caches;

public sealed class DataShareBinding<T>
{
    public string Name { get; }

    public Func<T> Getter { get; }

    public Action<T> Setter { get; }

    public DataShareBinding(
        string name,
        Func<T> getter,
        Action<T> setter)
    {
        this.Name = name;
        this.Getter = getter;
        this.Setter = setter;
    }
}
