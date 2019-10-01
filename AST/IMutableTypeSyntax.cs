namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMutableTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }
}
