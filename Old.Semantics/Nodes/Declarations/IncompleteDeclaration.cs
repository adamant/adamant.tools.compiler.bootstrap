using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Declarations
{
    public class IncompleteDeclaration : Declaration
    {
        public new IncompleteDeclarationSyntax Syntax { get; }

        public IncompleteDeclaration(IncompleteDeclarationSyntax syntax, IEnumerable<Diagnostic> diagnostics)
            : base(diagnostics)
        {
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
