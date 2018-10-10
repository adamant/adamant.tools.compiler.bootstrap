using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions
{
    public class InvocationExpression : Expression
    {
        public new InvocationSyntax Syntax { get; }
        public Expression Callee { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        public InvocationExpression(
            InvocationSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            DataType type,
            Expression callee,
            IEnumerable<Expression> arguments)
            : base(diagnostics, type)
        {
            Syntax = syntax;
            Callee = callee;
            Arguments = arguments.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
