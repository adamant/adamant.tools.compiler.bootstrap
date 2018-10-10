using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions
{
    public class UnknownExpression : Expression
    {
        public new SyntaxNode Syntax { get; }

        public UnknownExpression(SyntaxNode syntax, IEnumerable<Diagnostic> diagnostics)
            : base(diagnostics, DataType.Unknown)
        {
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
