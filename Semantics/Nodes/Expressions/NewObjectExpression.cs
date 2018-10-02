using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions
{
    public class NewObjectExpression : Expression
    {
        public new NewObjectExpressionSyntax Syntax { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        public NewObjectExpression(
            NewObjectExpressionSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            DataType type,
            IEnumerable<Expression> arguments)
            : base(diagnostics, type)
        {
            Syntax = syntax;
            Arguments = arguments.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax() => Syntax;

        public override void AllDiagnostics(IList<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            foreach (var argument in Arguments)
                argument.AllDiagnostics(list);
        }
    }
}
