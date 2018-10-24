using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class DeclarationAnalysisBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;
        [NotNull] private readonly ExpressionAnalysisBuilder expressionBuilder;
        [NotNull] private readonly StatementAnalysisBuilder statementBuilder;

        public DeclarationAnalysisBuilder(
            [NotNull] NameBuilder nameBuilder,
            [NotNull] ExpressionAnalysisBuilder expressionBuilder,
            [NotNull] StatementAnalysisBuilder statementBuilder)
        {
            this.nameBuilder = nameBuilder;
            this.statementBuilder = statementBuilder;
            this.expressionBuilder = expressionBuilder;
        }

        [CanBeNull]
        public MemberDeclarationAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    return Build(context, @namespace, function);
                case ClassDeclarationSyntax @class:
                    return Build(context, @namespace, @class);
                case IncompleteDeclarationSyntax _:
                    // Since it is incomplete, we can't do any analysis on it
                    return null;
                case EnumStructDeclarationSyntax enumStruct:
                    return Build(context, @namespace, enumStruct);
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private FunctionDeclarationAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] FunctionDeclarationSyntax syntax)
        {
            // Skip any function that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var bodyContext = context.InFunction(syntax);
            // For missing parameter names, use `_` to ignore them
            return new FunctionDeclarationAnalysis(
                context, syntax, fullName,
                syntax.Parameters.Select(p => new ParameterAnalysis(p, fullName.Qualify(p.Name.Value ?? "_"), expressionBuilder.Build(context, fullName, p.TypeExpression))),
                expressionBuilder.Build(context, fullName, syntax.ReturnTypeExpression),
                syntax.Body.Statements.Select(statementSyntax => statementBuilder.Build(bodyContext, fullName, statementSyntax)));
        }

        private static TypeDeclarationAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] ClassDeclarationSyntax syntax)
        {
            // Skip any class that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName);
        }

        private static TypeDeclarationAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] EnumStructDeclarationSyntax syntax)
        {
            // Skip any struct that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName);
        }
    }
}
