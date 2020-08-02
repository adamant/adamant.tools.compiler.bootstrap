using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new DataTypePromise ReturnDataType { get; }
    }
}
