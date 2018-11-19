using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders
{
    public class DeclarationAnalysisBuilder
    {
        [NotNull] private readonly IExpressionAnalysisBuilder expressionBuilder;
        [NotNull] private readonly IStatementAnalysisBuilder statementBuilder;

        public DeclarationAnalysisBuilder(
            [NotNull] IExpressionAnalysisBuilder expressionBuilder,
            [NotNull] IStatementAnalysisBuilder statementBuilder)
        {
            this.statementBuilder = statementBuilder;
            this.expressionBuilder = expressionBuilder;
        }

        [NotNull]
        public NamespaceDeclarationAnalysis BuildFileNamespace(
            [NotNull] AnalysisContext context,
            [NotNull] NamespaceDeclarationSyntax fileNamespace)
        {
            Requires.That(nameof(fileNamespace), fileNamespace.InGlobalNamespace);
            RootName name = GlobalNamespaceName.Instance;
            var namespaceContext = context;
            foreach (var ns in fileNamespace.Name)
            {
                var namespaceName = name.Qualify((SimpleName)ns);
                namespaceContext = BuildNamespaceContext(namespaceContext, fileNamespace, namespaceName);
                name = namespaceName;
            }

            var bodyContext = namespaceContext.WithUsingDirectives(fileNamespace);
            var declarations = new List<DeclarationAnalysis>();
            foreach (var declaration in fileNamespace.Declarations)
            {
                var analysis = BuildDeclaration(bodyContext, name, declaration);
                if (analysis != null)
                    declarations.Add(analysis);
            }
            return new NamespaceDeclarationAnalysis(context, fileNamespace, declarations);
        }

        [NotNull]
        private AnalysisContext BuildNamespaceContext(
            [NotNull] AnalysisContext context,
            [NotNull] NamespaceDeclarationSyntax @namespace,
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
                case TraitDeclarationSyntax trait:
                    return BuildTrait(context, @namespace, trait);
                case StructDeclarationSyntax @struct:
                    return BuildStruct(context, @namespace, @struct);
                case EnumStructDeclarationSyntax enumStruct:
                    return BuildEnumStruct(context, @namespace, enumStruct);
                case EnumClassDeclarationSyntax enumClass:
                    return BuildEnumClass(context, @namespace, enumClass);
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        [NotNull]
        private FunctionDeclarationAnalysis BuildFunction(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
            [NotNull] NamedFunctionDeclarationSyntax syntax)
        {
            var fullName = @namespace.Qualify(syntax.Name);
            var typesContext = syntax.GenericParameters != null ? context.WithGenericParameters(syntax) : context;
            var bodyContext = typesContext.InFunctionBody(syntax);
            // For missing parameter names, use `_` to ignore them
            return new FunctionDeclarationAnalysis(
                context, syntax, fullName, BuildGenericParameters(context, fullName, syntax.GenericParameters),
                syntax.Parameters?.Select(p => BuildParameter(typesContext, fullName, p)),
                 expressionBuilder.BuildExpression(typesContext, fullName, syntax.ReturnType),
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
                    return new ParameterAnalysis(context, namedParameter, functionName.Qualify(namedParameter.Name),
                        expressionBuilder.BuildExpression(context, functionName, namedParameter.Type));
                case SelfParameterSyntax selfParameter:
                    return new ParameterAnalysis(context, selfParameter, functionName.Qualify(new SimpleName("self", true)), null);
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }

        [NotNull]
        private TypeDeclarationAnalysis BuildClass(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
            [NotNull] ClassDeclarationSyntax syntax)
        {
            var fullName = @namespace.Qualify(syntax.Name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [NotNull]
        private TypeDeclarationAnalysis BuildTrait(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
            [NotNull] TraitDeclarationSyntax syntax)
        {
            var fullName = @namespace.Qualify(syntax.Name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [NotNull]
        private TypeDeclarationAnalysis BuildStruct(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
            [NotNull] StructDeclarationSyntax syntax)
        {
            switch (syntax.Name)
            {
                case IIdentifierToken identifier:
                {
                    var fullName = @namespace.Qualify((SimpleName)identifier.Value);
                    return new TypeDeclarationAnalysis(context, syntax, fullName,
                        BuildGenericParameters(context, fullName, syntax.GenericParameters));
                }
                case IPrimitiveTypeToken primitive:
                {
                    var name = new SimpleName(context.File.Code[primitive.Span], true);
                    return new TypeDeclarationAnalysis(context, syntax,
                        name,
                        BuildGenericParameters(context, name, syntax.GenericParameters));
                }
                default:
                    throw NonExhaustiveMatchException.For(syntax.Name);
            }
        }

        [NotNull]
        private TypeDeclarationAnalysis BuildEnumStruct(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
            [NotNull] EnumStructDeclarationSyntax syntax)
        {
            var fullName = @namespace.Qualify(syntax.Name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [NotNull]
        private TypeDeclarationAnalysis BuildEnumClass(
            [NotNull] AnalysisContext context,
            [NotNull] RootName @namespace,
            [NotNull] EnumClassDeclarationSyntax syntax)
        {
            var fullName = @namespace.Qualify(syntax.Name);
            return new TypeDeclarationAnalysis(context, syntax, fullName,
                BuildGenericParameters(context, fullName, syntax.GenericParameters));
        }

        [CanBeNull, ItemNotNull]
        private IEnumerable<GenericParameterAnalysis> BuildGenericParameters(
            [NotNull] AnalysisContext context,
            [NotNull] Name memberName,
            [CanBeNull] FixedList<GenericParameterSyntax> syntax)
        {
            return syntax?.Select(parameter => new GenericParameterAnalysis(context,
                parameter.NotNull(),
                memberName.Qualify(parameter.NotNull().Name),
                parameter.NotNull().Type == null
                    ? null
                    : expressionBuilder.BuildExpression(context, memberName,
                        parameter.NotNull().Type)));
        }
    }
}
