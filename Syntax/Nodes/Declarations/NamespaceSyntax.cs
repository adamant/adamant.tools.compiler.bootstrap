using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class NamespaceSyntax : DeclarationSyntax
    {
        public override IdentifierToken Name => throw new NotImplementedException();

        public NamespaceSyntax(IEnumerable<DeclarationSyntax> children)
            : base(children)
        {
        }
    }
}
