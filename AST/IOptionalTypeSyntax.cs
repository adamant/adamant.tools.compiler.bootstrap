namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }
}
