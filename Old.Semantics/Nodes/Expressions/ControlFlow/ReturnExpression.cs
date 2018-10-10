using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions.ControlFlow
{
    public class ReturnExpression : Expression
    {
        [NotNull] public new ReturnExpressionSyntax Syntax { get; }
        [CanBeNull] public Expression Expression { get; }

        public ReturnExpression(
            [NotNull] ReturnExpressionSyntax syntax,
            [NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [CanBeNull] Expression expression)
        // TODO take in the type rather than assuming never here (let that be semantic analysis)
            : base(diagnostics, PrimitiveType.Never)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Expression = expression;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics([NotNull][ItemNotNull] List<Diagnostic> list)
        {
            Requires.NotNull(nameof(list), list);
            base.AllDiagnostics(list);
            Expression?.AllDiagnostics(list);
        }
    }
}
