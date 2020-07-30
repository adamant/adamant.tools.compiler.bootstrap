using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax TypeSyntax { get; }
        ICallableNameSyntax? ConstructorName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
        [DisallowNull] IFunctionMetadata? ReferencedConstructor { get; set; }
    }
}
