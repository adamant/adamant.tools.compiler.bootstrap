using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface ISelfExpressionSyntax : IExpressionSyntax, IHasContainingScope
    {
        bool IsImplicit { get; }
        [DisallowNull] IBindingMetadata? ReferencedBinding { get; set; }
        FixedList<IMetadata> LookupInContainingScope();
    }
}
