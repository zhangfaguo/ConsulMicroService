namespace Zfg.Core.Common.Piples
{
    public interface IPiple<TContent>
        where TContent : IPipleContent
    {

        void Excute(IScope Scope, TContent content);
    }
}
