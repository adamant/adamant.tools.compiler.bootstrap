using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// An expression that is a single unqualified name
    /// </summary>
    public partial interface INameExpressionSyntax
    {
        IEnumerable<IPromise<BindingSymbol>> LookupInContainingScope();
        bool VariableIsLiveAfter { get; set; }
    }
}
