using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations.Function;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Namespaces;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class DeclarationAnalysisBuilder
    {
        [NotNull] private readonly IExpressionAnalysisBuilder expressionBuilder;
        [NotNull] private readonly IStatementAnalysisBuilder statementBuilder;
        [NotNull] private readonly NameBuilder nameBuilder;

        public DeclarationAnalysisBuilder(
            [NotNull] IExpressionAnalysisBuilder expressionBuilder,
            [NotNull] IStatementAnalysisBuilder statementBuilder,
            [NotNull] NameBuilder nameBuilder)
        {
            this.statementBuilder = statementBuilder;
            this.nameBuilder = nameBuilder;
            this.expressionBuilder = expressionBuilder;
        }

        [NotNull]
        public NamespaceDeclarationAnalysis BuildFileNamespace(
            [NotNull] AnalysisContext context,
            [NotNull] FileNamespaceDeclarationSyntax @namespace)
        {
            Name name = GlobalNamespaceName.Instance;
            if (@namespace.Name != null)
                name = nameBuilder.BuildName(@namespace.Name) ?? name;
            var bodyContext = context.InNamespace(@namespace);

            var declarations = new List<DeclarationAnalysis>();
            foreach (var declaration in @namespace.Declarations)
            {
                var analysis = BuildDeclaration(bodyContext, name, declaration);
                if (analysis != null)
                    declarations.Add(analysis);
            }
            return new NamespaceDeclarationAnalysis(context, @namespace, declarations);
        }

        [CanBeNull]
        public MemberDeclarationAnalysis BuildDeclaration(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case NamedFunctionDeclarationSyntax function:
                    return BuildFunction(context, @namespace, function);
                case ClassDeclarationSyntax @class:
                    return BuildClass(context, @namespace, @class);
                case TypeDeclarationSyntax type:
                    return BuildType(context, @namespace, type);
                case StructDeclarationSyntax @struct:
                    return BuildStruct(context, @namespace, @struct);
                case EnumStructDeclarationSyntax enumStruct:
                    return BuildEnumStruct(context, @namespace, enumStruct);
                case EnumClassDeclarationSyntax enumClass:
                    return BuildEnumClass(context, @namespace, enumClass);
                case IncompleteDeclarationSyntax _:
                    // Since it is incomplete, we can't do any analysis on it
                    return null;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        [CanBeNull]
        private FunctionDeclarationAnalysis BuildFunction(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] NamedFunctionDeclarationSyntax syntax)
        {
            // Skip any function that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var typesContext = context.WithGenericParameters(syntax);
            var bodyContext = typesContext.InFunctionBody(syntax);
            // For missing parameter names, use `_` to ignore them
            return new FunctionDeclarationAnalysis(
                context, syntax, fullName, BuildGenericParameters(context, fullName, syntax.GenericParameters),
                syntax.Parameters.Select(p => BuildParameter(typesContext, fullName, p)),
                expressionBuilder.BuildExpression(typesContext, fullName, syntax.ReturnTypeExpression),
                syntax.Body?.Statements.Select(statementSyntax => statementBuilder.Build(bodyContext, fullName, statementSyntax)));
        }

        [NotNull]
        private ParameterAnalysis BuildParameter(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] ParameterSyntax parameter)
        {
            switch (parameter)
            {
                case NamedParameterSyntax namedParameter:
                    return new ParameterAnalysis(context, namedParameter, functionName.Qualify(namedParameter.Name.Value ?? "_"),
                        expressionBuilder.BuildExpression(context, functionName, namedParameter.TypeExpression));
                case SelfParameterSyntax selfParameter:
                    return new ParameterAnalysis(context, selfParameter, functionName.Qualify(new SimpleName("self", true)), null);
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }

        [CanBeNull]
        private TypeDeclarationAnalysis BuildClass(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] ClassDeclarationSyntax syntax)
        {
            // Skip any class that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [CanBeNull]
        private TypeDeclarationAnalysis BuildType(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] TypeDeclarationSyntax syntax)
        {
            // Skip any class that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [CanBeNull]
        private TypeDeclarationAnalysis BuildStruct(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] StructDeclarationSyntax syntax)
        {
            switch (syntax.Name)
            {
                case IdentifierToken identifier:
                    {
                        var fullName = @namespace.Qualify(identifier.Value);
                        return new TypeDeclarationAnalysis(context, syntax, fullName,
                            BuildGenericParameters(context, fullName, syntax.GenericParameters));
                    }
                case MissingToken _:
                default:
                    // Skip any struct that doesn't have a name
                    return null;
                case IPrimitiveTypeToken primitive:
                    {
                        var name = new SimpleName(context.File.Code[primitive.Span], true);
                        var fullName = new QualifiedName(name);
                        return new TypeDeclarationAnalysis(context, syntax,
                            fullName,
                            BuildGenericParameters(context, fullName, syntax.GenericParameters));
                    }
            }
        }

        [CanBeNull]
        private TypeDeclarationAnalysis BuildEnumStruct(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] EnumStructDeclarationSyntax syntax)
        {
            // Skip any struct that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [CanBeNull]
        private TypeDeclarationAnalysis BuildEnumClass(
            [NotNull] AnalysisContext context,
            [NotNull] Name @namespace,
            [NotNull] EnumClassDeclarationSyntax syntax)
        {
            // Skip any struct that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<GenericParameterAnalysis> BuildGenericParameters(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName memberName,
            [CanBeNull] GenericParametersSyntax syntax)
        {
            if (syntax == null) yield break;
            foreach (var parameter in syntax.Parameters)
            {
                yield return new GenericParameterAnalysis(context, parameter,
                    memberName.Qualify(parameter.Name.Value ?? "_"),
                    parameter.TypeExpression == null ? null : expressionBuilder.BuildExpression(context, memberName, parameter.TypeExpression));
            }
        }
    }
}
