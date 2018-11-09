using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class ModifierSyntax : NonTerminal
    {
        public abstract IEnumerable<IToken> Tokens();
    }
}
