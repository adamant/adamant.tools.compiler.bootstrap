using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFunctionDeclarationSyntax
    {
        new FixedList<INamedParameterSyntax> Parameters { get; }
    }
}
