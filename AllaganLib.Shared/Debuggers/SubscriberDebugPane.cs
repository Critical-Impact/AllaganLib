using System;
using System.Collections.Generic;
using AllaganLib.Shared.Interfaces;

namespace AllaganLib.Shared.Debuggers;

public abstract class SubscriberDebugPane : IDebugPane, IDisposable
{
    private readonly List<Action> unsubscribeActions = new();

    /// <inheritdoc/>
    public abstract string Name { get; }

    /// <summary>
    /// Concrete implementations should set up subscriptions here,
    /// calling <see cref="RegisterSubscription"/> for each event.
    /// </summary>
    public virtual void SubscribeToEvents()
    {

    }


    /// <inheritdoc/>
    public abstract void Draw();

    /// <summary>
    /// Helper to register an event subscription with an associated unsubscriber.
    /// </summary>
    protected void RegisterSubscription(Action? unsubscribeAction)
    {
        if (unsubscribeAction != null)
        {
            this.unsubscribeActions.Add(unsubscribeAction);
        }
    }

    /// <summary>
    /// Initialize subscriptions (call this after construction).
    /// </summary>
    public void Initialize()
    {
        this.SubscribeToEvents();
    }

    /// <summary>
    /// Unsubscribes from all registered event handlers.
    /// </summary>
    public void UnsubscribeAll()
    {
        foreach (var unsubscribe in this.unsubscribeActions)
        {
            unsubscribe();
        }

        this.unsubscribeActions.Clear();
    }

    public void Dispose()
    {
        this.UnsubscribeAll();
    }
}