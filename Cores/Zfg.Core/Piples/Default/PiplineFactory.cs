using System;

namespace Zfg.Core.Common.Piples
{
    public class PiplineFactory<T> : IPipleFactory<T>
        where T : IPipleContent
    {
        public IPiple<T> Create(Action<IScope, T> action)
        {
            return new Pipline<T>(action);
        }
    }
}
