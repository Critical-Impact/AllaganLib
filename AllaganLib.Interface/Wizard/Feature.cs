using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.Interface.FormFields;

namespace AllaganLib.Interface.Wizard;

public abstract class Feature<T> : IFeature<T>
{
    public Feature(IEnumerable<Type> applicableSettings, IEnumerable<IFormField<T>> settings)
    {
        var settingsAsList = settings.ToList();
        var settingsHashSet = applicableSettings.ToHashSet();
        this.RelatedSettings = new List<IFormField<T>>();
        this.RelatedSettings = settingsAsList.Where(c => settingsHashSet.Contains(c.GetType())).OrderByDescending(c => settingsAsList.IndexOf(c)).ToList();
    }

    public List<IFormField<T>> RelatedSettings { get; }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public virtual void OnFinish()
    {
    }
}