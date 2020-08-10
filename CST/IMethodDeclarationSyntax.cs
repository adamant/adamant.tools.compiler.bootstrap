using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IMethodDeclarationSyntax : IMethodMetadata
    {
        new FixedList<INamedParameterSyntax> Parameters { get; }
    }
}
