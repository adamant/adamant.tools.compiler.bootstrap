using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface INewObjectExpressionSyntax
    {
        [DisallowNull] IFunctionMetadata? ReferencedConstructor { get; set; }
    }
}
