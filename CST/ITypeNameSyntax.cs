using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ITypeNameSyntax
    {
        IEnumerable<IPromise<Symbol>> LookupInContainingScope();
    }
}
