using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers
{
    public abstract class ModifierSyntax : SyntaxNode
    {
        public abstract IEnumerable<IToken> Tokens();
    }
}