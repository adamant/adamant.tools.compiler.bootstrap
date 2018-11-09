using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders
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
            RootName name = GlobalNamespaceName.Instance;
            var namespaceContext = context;
            if (@namespace.Name != null)
            {
                var namespaceName = nameBuilder.BuildName(@namespace.Name);
                if (namespaceName != null)
                {
                    namespaceContext = BuildNamespaceContext(namespaceContext, @namespace, namespaceName);
                    name = namespaceName;
                }
            }
            var bodyContext = namespaceContext.WithUsingDirectives(@namespace);
            var declarations = new List<DeclarationAnalysis>();
            foreach (var declaration in @namespace.Declarations)
            {
                var analysis = BuildDeclaration(bodyContext, name, declaration);
                if (analysis != null)
                    declarations.Add(analysis);
            }
            return new NamespaceDeclarationAnalysis(context, @namespace, declarations);
        }

        [NotNull]
        private AnalysisContext BuildNamespaceContext(
            [NotNull] AnalysisContext context,
            [NotNull] FileNamespaceDeclarationSyntax @namespace,
            [NotNull] Name name)
        {
            switch (name)
            {
                case SimpleName simpleName:
                    return context.InNamespace(@namespace, simpleName);
                case QualifiedName qualifiedName:
                    context = BuildNamespaceContext(context, @namespace, qualifiedName.Qualifier);
                    return context.InNamespace(@namespace, qualifiedName);
                default:
                    throw NonExhaustiveMatchException.For(name);
            }
        }

        [CanBeNull]
        public MemberDeclarationAnalysis BuildDeclaration(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
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
            [NotNull] RootName @namespace,
            [NotNull] NamedFunctionDeclarationSyntax syntax)
        {
            // Skip any function that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var typesContext = syntax.GenericParameters != null ? context.WithGenericParameters(syntax) : context;
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
            [NotNull] Name functionName,
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
            [NotNull] RootName @namespace,
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
            [NotNull] RootName @namespace,
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
            [NotNull] RootName @namespace,
            [NotNull] StructDeclarationSyntax syntax)
        {
            switch (syntax.Name)
            {
                case IIdentifierToken identifier:
                {
                    var fullName = @namespace.Qualify(identifier.Value);
                    return new TypeDeclarationAnalysis(context, syntax, fullName,
                        BuildGenericParameters(context, fullName, syntax.GenericParameters));
                }
                case IMissingToken _:
                default:
                    // Skip any struct that doesn't have a name
                    return null;
                case IPrimitiveTypeToken primitive:
                {
                    var name = new SimpleName(context.File.Code[primitive.Span], true);
                    return new TypeDeclarationAnalysis(context, syntax,
                        name,
                        BuildGenericParameters(context, name, syntax.GenericParameters));
                }
            }
        }

        [CanBeNull]
        private TypeDeclarationAnalysis BuildEnumStruct(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
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
            [NotNull] RootName @namespace,
            [NotNull] EnumClassDeclarationSyntax syntax)
        {
            // Skip any struct that doesn't have a name
            if (!(syntax.Name.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [CanBeNull, ItemNotNull]
        private IEnumerable<GenericParameterAnalysis> BuildGenericParameters(
            [NotNull] AnalysisContext context,
            [NotNull] Name memberName,
            [CanBeNull] GenericParametersSyntax syntax)
        {
            return syntax?.Parameters.Select(parameter => new GenericParameterAnalysis(context,
                parameter,
                memberName.Qualify(parameter.Name.Value ?? "_"),
                parameter.TypeExpression == null
                    ? null
                    : expressionBuilder.BuildExpression(context, memberName,
                        parameter.TypeExpression)));
        }
    }
}
