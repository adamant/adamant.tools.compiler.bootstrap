using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IConstructorDeclarationSyntax
    {
        new FixedList<IConstructorParameterSyntax> Parameters { get; }
    }
}
