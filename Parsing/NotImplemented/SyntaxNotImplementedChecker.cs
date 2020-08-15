using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.NotImplemented
{
    /// <summary>
    /// Reports errors for syntax that the parsing supports but the semantic
    /// analyzer doesn't.
    /// </summary>
    internal class SyntaxNotImplementedChecker : SyntaxWalker
    {
        private readonly CompilationUnitSyntax compilationUnit;
        private readonly Diagnostics diagnostics;
        private readonly CodeFile file;

        public SyntaxNotImplementedChecker(CompilationUnitSyntax compilationUnit, Diagnostics diagnostics)
        {
            this.compilationUnit = compilationUnit;
            this.diagnostics = diagnostics;
            file = compilationUnit.File;
        }

        public void Check()
        {
            WalkNonNull(compilationUnit);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case INamedParameterSyntax syn:
                    if (!(syn.DefaultValue is null))
                        diagnostics.Add(ParseError.NotImplemented(file, syn.DefaultValue.Span, "Default values"));
                    break;
                case IFieldParameterSyntax syn:
                    if (!(syn.DefaultValue is null))
                        diagnostics.Add(ParseError.NotImplemented(file, syn.DefaultValue.Span, "Default values"));
                    break;
                case IConstructorDeclarationSyntax syn:
                    if (!(syn.Name is null))
                        diagnostics.Add(ParseError.NotImplemented(file, syn.Span, "Named constructors"));
                    break;
                case IFieldDeclarationSyntax syn:
                    if (!(syn.Initializer is null))
                        diagnostics.Add(ParseError.NotImplemented(file, syn.Initializer.Span, "Field initializers"));
                    break;
                case IForeachExpressionSyntax syn:
                {
                    var inExpression = syn.InExpression;
                    if (!(inExpression is IBinaryOperatorExpressionSyntax exp)
                        || (exp.Operator != BinaryOperator.DotDot
                           && exp.Operator != BinaryOperator.LessThanDotDot
                           && exp.Operator != BinaryOperator.DotDotLessThan
                           && exp.Operator != BinaryOperator.LessThanDotDotLessThan))
                        diagnostics.Add(ParseError.NotImplemented(file, inExpression.Span, "Foreach in non range expressions"));
                }
                break;
            }
            WalkChildren(syntax);
        }
    }
}
