using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IMethodDeclarationSyntax : IMethodMetadata
    {
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
    }
}
