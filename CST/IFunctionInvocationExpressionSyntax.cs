using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFunctionInvocationExpressionSyntax
    {
        IEnumerable<IPromise<FunctionSymbol>> LookupInContainingScope();
    }
}
