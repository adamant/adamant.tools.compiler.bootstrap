using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Void = Adamant.Tools.Compiler.Bootstrap.Framework.Void;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class ReferencedSymbolValidator : DeclarationVisitor<Void>
    {
        public static void Validate(IEnumerable<IEntityDeclarationSyntax> entityDeclaration)
        {
            var validator = new ReferencedSymbolValidator();
            foreach (var declaration in entityDeclaration)
                validator.VisitDeclaration(declaration);
        }

        private static void AssertHasReferencedSymbol(
            IExpressionSyntax expression,
            ISymbol referencedSymbol)
        {
            if (referencedSymbol == null)
                throw new Exception($"Expression doesn't have referenced symbol `{expression}`");
        }

        public override void VisitMemberAccessExpression(IMemberAccessExpressionSyntax memberAccessExpression, Void args)
        {
            base.VisitMemberAccessExpression(memberAccessExpression, args);
            AssertHasReferencedSymbol(memberAccessExpression, memberAccessExpression.ReferencedSymbol);
        }

        public override void VisitName(INameSyntax name, Void args)
        {
            base.VisitName(name, args);
            AssertHasReferencedSymbol(name, name.ReferencedSymbol);
        }
    }
}
