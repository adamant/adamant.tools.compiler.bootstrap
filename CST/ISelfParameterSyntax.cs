namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ISelfParameterSyntax : IParameterSyntax
    {
        bool MutableSelf { get; }
    }
}
