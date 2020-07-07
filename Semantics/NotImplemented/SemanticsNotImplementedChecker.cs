using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NotImplemented
{
    /// <summary>
    /// Reports errors for syntax that the parsing supports but the semantic
    /// analyzer doesn't.
    /// </summary>
    public class SemanticsNotImplementedChecker : SyntaxWalker
    {
        private readonly Diagnostics diagnostics;
        private CodeFile file;

        public SemanticsNotImplementedChecker(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
            file = null!;
        }

        public void Check(PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                file = compilationUnit.CodeFile;
                WalkNonNull(compilationUnit);
            }
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case INamedParameterSyntax syn:
                    if (!(syn.DefaultValue is null))
                        diagnostics.Add(SemanticError.NotImplemented(file, syn.DefaultValue.Span, "Default values"));
                    break;
                case IFieldParameterSyntax syn:
                    if (!(syn.DefaultValue is null))
                        diagnostics.Add(SemanticError.NotImplemented(file, syn.DefaultValue.Span, "Default values"));
                    break;
                case IConstructorDeclarationSyntax syn:
                    if (syn.Name != SpecialName.New)
                        diagnostics.Add(SemanticError.NotImplemented(file, syn.Span, "Named constructors"));
                    break;
                case IClassDeclarationSyntax syn:
                    // TODO classes need to be checked for duplicate modifiers
                    CheckMoveModifier(syn);
                    CheckSafeUnsafeModifiers(syn);
                    break;
                case IFieldDeclarationSyntax syn:
                    if (!(syn.Initializer is null))
                        diagnostics.Add(SemanticError.NotImplemented(file, syn.Initializer.Span, "Field initializers"));
                    CheckMoveModifier(syn);
                    CheckSafeUnsafeModifiers(syn);
                    break;
                case IEntityDeclarationSyntax syn:
                    CheckMutableModifier(syn);
                    CheckMultipleModifiers(syn);
                    CheckMoveModifier(syn);
                    CheckSafeUnsafeModifiers(syn);
                    break;
                case IForeachExpressionSyntax syn:
                {
                    var inExpression = syn.InExpression;
                    if (!(inExpression is IBinaryOperatorExpressionSyntax exp)
                        || (exp.Operator != BinaryOperator.DotDot
                           && exp.Operator != BinaryOperator.LessThanDotDot
                           && exp.Operator != BinaryOperator.DotDotLessThan
                           && exp.Operator != BinaryOperator.LessThanDotDotLessThan))
                        diagnostics.Add(SemanticError.NotImplemented(file, inExpression.Span, "Foreach in non range expressions"));
                }
                break;
            }
            WalkChildren(syntax);
        }

        private void CheckMultipleModifiers(IEntityDeclarationSyntax syn)
        {
            if (syn.Modifiers.Count > 1) diagnostics.Add(SemanticError.NotImplemented(file, syn.Span, "Multiple modifiers"));
        }

        private void CheckMutableModifier(IEntityDeclarationSyntax syn)
        {
            if (syn.Modifiers.OfType<IMutableKeywordToken>().Any())
                diagnostics.Add(SemanticError.NotImplemented(file, syn.Span, "`mut` modifiers"));
        }

        private void CheckSafeUnsafeModifiers(IEntityDeclarationSyntax syn)
        {
            if (syn.Modifiers.OfType<ISafeKeywordToken>().Any())
                diagnostics.Add(SemanticError.NotImplemented(file, syn.Span, "`safe` modifiers"));
            if (syn.Modifiers.OfType<IUnsafeKeywordToken>().Any())
                diagnostics.Add(SemanticError.NotImplemented(file, syn.Span, "`unsafe` modifiers"));
        }

        private void CheckMoveModifier(IEntityDeclarationSyntax syn)
        {
            if (syn.Modifiers.OfType<IMoveKeywordToken>().Any())
                diagnostics.Add(SemanticError.NotImplemented(file, syn.Span, "`move` modifiers"));
        }
    }
}
