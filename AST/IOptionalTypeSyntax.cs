namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }
}
