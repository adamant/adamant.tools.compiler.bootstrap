using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NameBinding
{
    public class ExpressionNameBinder : ExpressionVisitor<LexicalScope, Void>
    {
        [NotNull] private readonly Diagnostics diagnostics;
        [NotNull] private readonly CodeFile file;

        public ExpressionNameBinder(
            [NotNull] Diagnostics diagnostics,
            [NotNull] CodeFile file)
        {
            this.diagnostics = diagnostics;
            this.file = file;
        }

        public override Void VisitExpression([CanBeNull] ExpressionSyntax expression, [NotNull] LexicalScope containingScope)
        {
            return expression == null ? default : base.VisitExpression(expression, containingScope);
        }

        public override Void VisitBlock([CanBeNull] BlockSyntax block, [NotNull] LexicalScope containingScope)
        {
            if (block == null) return default;

            var symbols = new List<ISymbol>();

            foreach (var variableDeclaration in block.Statements
                .OfType<VariableDeclarationStatementSyntax>())
                symbols.Add(variableDeclaration);

            containingScope = new NestedScope(containingScope, symbols);

            foreach (var statement in block.Statements)
                VisitStatement(statement, containingScope);

            return default;
        }

        public override Void VisitIdentifierName(
            [NotNull] IdentifierNameSyntax identifierName,
            [NotNull] LexicalScope containingScope)
        {
            var symbol = containingScope.Lookup(identifierName.Name);
            if (symbol == null)
                diagnostics.Add(NameBindingError.CouldNotBindName(file, identifierName.Span));
            identifierName.ReferencedSymbol = symbol ?? UnknownSymbol.Instance;
            return default;
        }
    }
}
