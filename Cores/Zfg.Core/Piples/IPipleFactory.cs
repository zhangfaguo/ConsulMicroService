using System;

namespace Zfg.Core.Common.Piples
{
    public interface IPipleFactory<TContent>
        where TContent : IPipleContent
    {

        IPiple<TContent> Create(Action<IScope, TContent> action);
    }
}
