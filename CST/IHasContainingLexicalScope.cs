using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IHasContainingLexicalScope
    {
        LexicalScope<IPromise<Symbol>> ContainingLexicalScope { get; set; }
    }
}
