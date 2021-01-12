namespace Zfg.Core.Common.Piples
{
    public interface IPipelineStep<TContent>
        where TContent : IPipleContent
    {

        void Handle(IScope scope, TContent content);

    }
}
