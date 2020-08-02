namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IOptionalTypeSyntax : ITypeSyntax
    {
        ITypeSyntax Referent { get; }
    }
}
