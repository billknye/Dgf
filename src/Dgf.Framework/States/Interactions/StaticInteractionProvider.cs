using System;
using System.Collections.Generic;

namespace Dgf.Framework.States.Interactions;

public abstract class StaticInteractionProvider<TGameState> : InteractionProvider<TGameState> where TGameState : IInteractionGameState, new()
{
    protected List<InteractionProvider<TGameState>> staticChildProviders;

    public StaticInteractionProvider()
    {
        staticChildProviders = new List<InteractionProvider<TGameState>>();
        foreach (var provider in GetStaticProviders())
        {
            staticChildProviders.Add(provider);
        }
    }

    protected Action<TGameState> TransitionTo<T>(Action<TGameState> action = null)
    {
        for (int i = 0; i < staticChildProviders.Count; i++)
        {
            if (staticChildProviders[i] is T)
                return n =>
                {
                    n.States.Push(i);
                    action?.Invoke(n);
                };
        }

        throw new InvalidOperationException();
    }

    protected abstract IEnumerable<InteractionProvider<TGameState>> GetStaticProviders();

    protected override IEnumerable<InteractionProvider<TGameState>> GetChildProviders(TGameState state) => staticChildProviders;
}

