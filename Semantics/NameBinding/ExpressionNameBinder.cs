using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NameBinding
{
    public class ExpressionNameBinder : ExpressionVisitor<LexicalScope>
    {
        public override void VisitBlock([CanBeNull] BlockSyntax block, [NotNull] LexicalScope containingScope)
        {
            if (block == null) return;

            var symbols = new List<ISymbol>();

            foreach (var variableDeclaration in block.Statements
                .OfType<VariableDeclarationStatementSyntax>())
                symbols.Add(variableDeclaration);

            containingScope = new NestedScope(containingScope, symbols);

            foreach (var statement in block.Statements)
                VisitStatement(statement, containingScope);
        }

        public override void VisitIdentifierName(
            [NotNull] IdentifierNameSyntax identifierName,
            [NotNull] LexicalScope containingScope)
        {
            identifierName.ContainingScope = containingScope;
        }
    }
}
