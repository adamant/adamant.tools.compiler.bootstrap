using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions
{
    public class NewObjectExpression : Expression
    {
        [NotNull] public new NewObjectExpressionSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Expression> Arguments { get; }

        public NewObjectExpression(
            [NotNull] NewObjectExpressionSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [NotNull] DataType type,
            [NotNull] [ItemNotNull] IEnumerable<Expression> arguments)
            : base(diagnostics, type)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(arguments), arguments);
            Syntax = syntax;
            Arguments = arguments.ToList().AsReadOnly().AssertNotNull();
        }

        [NotNull]
        protected override SyntaxNode GetSyntax() => Syntax;

        public override void AllDiagnostics([NotNull] List<Diagnostic> list)
        {
            Requires.NotNull(nameof(list), list);
            base.AllDiagnostics(list);
            foreach (var argument in Arguments)
                argument.AllDiagnostics(list);
        }
    }
}
