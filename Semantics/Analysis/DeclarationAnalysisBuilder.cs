using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations.Function;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Function;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class DeclarationAnalysisBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;
        [NotNull] private readonly IExpressionAnalysisBuilder expressionBuilder;
        [NotNull] private readonly IStatementAnalysisBuilder statementBuilder;

        public DeclarationAnalysisBuilder(
            [NotNull] NameBuilder nameBuilder,
            [NotNull] IExpressionAnalysisBuilder expressionBuilder,
            [NotNull] IStatementAnalysisBuilder statementBuilder)
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
                syntax.Parameters.Select(p => Build(context, fullName, p)),
                expressionBuilder.Build(context, fullName, syntax.ReturnTypeExpression),
                syntax.Body.Statements.Select(statementSyntax => statementBuilder.Build(bodyContext, fullName, statementSyntax)));
        }

        private ParameterAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] ParameterSyntax parameter)
        {
            switch (parameter)
            {
                case NamedParameterSyntax namedParameter:
                    return new ParameterAnalysis(namedParameter, functionName.Qualify(namedParameter.Name.Value ?? "_"), expressionBuilder.Build(context, functionName, namedParameter.TypeExpression));
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
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
