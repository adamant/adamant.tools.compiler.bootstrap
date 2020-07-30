namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }
}
