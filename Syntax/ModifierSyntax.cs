using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class ModifierSyntax : SyntaxNode
    {
        public abstract IEnumerable<IToken> Tokens();
    }
}
