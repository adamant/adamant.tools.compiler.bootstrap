using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// An expression that is a single unqualified name
    /// </summary>
    public partial interface INameExpressionSyntax
    {
        IEnumerable<IPromise<BindingSymbol>> LookupInContainingScope();
        [DisallowNull] IBindingMetadata? ReferencedBinding { get; set; }
        FixedList<IMetadata> LookupMetadataInContainingScope();
        bool VariableIsLiveAfter { get; set; }
    }
}
