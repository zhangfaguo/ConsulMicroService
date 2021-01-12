using System;
using System.Collections.Generic;

namespace Zfg.Core.Common.Piples
{
    public class PipleBuilder<TPipleContent>
        where TPipleContent : IPipleContent
    {

        List<IPipelineStep<TPipleContent>> steps;

        IPipleFactory<TPipleContent> pipleFactory;
        public PipleBuilder(IPipleFactory<TPipleContent> factory)
        {
            steps = new List<IPipelineStep<TPipleContent>>();
            pipleFactory = factory;
        }

        public void Add(IPipelineStep<TPipleContent> step)
        {
            steps.Add(step);
        }

        public IPiple<TPipleContent> Build()
        {
            steps.Reverse();
            Action<Action<IScope, TPipleContent>, IScope, TPipleContent> next = (t, s, c) =>
            {
                if (t != null)
                {
                    t(s, c);
                }
            };
            Action<IScope, TPipleContent> pipe = null;
            foreach (var item in steps)
            {
                var t = pipe;
                pipe = (s, c) =>
                {
                    item.Handle(s, c);
                    if (c.IsBreak)
                    {
                        return;
                    }

                    next(t, s, c);
                };
            }
            return pipleFactory.Create(pipe);
        }

    }
}
