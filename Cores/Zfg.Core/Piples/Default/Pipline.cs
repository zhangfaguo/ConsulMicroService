using System;

namespace Zfg.Core.Common.Piples
{
    public class Pipline<T> : IPiple<T>
        where T : IPipleContent
    {
        Action<IScope, T> action;

        public Pipline(Action<IScope, T> act)
        {
            action = act;
        }

        public void Excute(IScope Scope, T content)
        {
            action?.Invoke(Scope, content);
        }
    }
}
