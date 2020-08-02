using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IAssociatedFunctionDeclarationSyntax
    {
        new FixedList<INamedParameterSyntax> Parameters { get; }
    }
}
