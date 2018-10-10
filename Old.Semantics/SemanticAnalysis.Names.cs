using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string NameAttribute = "Name";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackageName Name([NotNull] PackageSyntax s) => Name<PackageName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GlobalNamespaceName Name([NotNull] CompilationUnitSyntax s) => Name<GlobalNamespaceName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionName Name([NotNull] FunctionDeclarationSyntax s) => Name<FunctionName>(s);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectTypeName Name([NotNull] ClassDeclarationSyntax s) => Name<ObjectTypeName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectTypeName Name([NotNull] EnumStructDeclarationSyntax s) => Name<ObjectTypeName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VariableName Name([NotNull] ParameterSyntax s) => Name<VariableName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Name Name([NotNull] IdentifierNameSyntax s) => Name<Name>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Name Name([NotNull] SyntaxNode syntax)
        {
            return attributes.GetOrAdd(syntax, NameAttribute, ComputeName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TName Name<TName>([NotNull] SyntaxNode syntax)
            where TName : Name
        {
            return (TName)attributes.GetOrAdd(syntax, NameAttribute, ComputeName);
        }

        private Name ComputeName([NotNull] SyntaxNode syntax)
        {
            switch (syntax)
            {
                case PackageSyntax package:
                    // TODO get actual package name
                    return new PackageName("default");

                case CompilationUnitSyntax compilationUnit:
                    return GlobalNamespaceName(Parent(compilationUnit));

                case ClassDeclarationSyntax classDeclaration:
                    {
                        var parentName = Name(Parent(classDeclaration));
                        return new ReferenceTypeName(parentName, classDeclaration.Name.Value);
                    }
                case EnumStructDeclarationSyntax enumDeclaration:
                    {
                        var parentName = Name(Parent(enumDeclaration));
                        return new ReferenceTypeName(parentName, enumDeclaration.Name.Value);
                    }
                case FunctionDeclarationSyntax functionDeclaration:
                    {
                        var parentName = Name(Parent(functionDeclaration));
                        return new FunctionName(parentName, functionDeclaration.Name.Value,
                            functionDeclaration.Parameters.Count());
                    }
                case ParameterSyntax parameter:
                    {
                        var scope = LexicalScope(parameter);
                        var functionSyntax = (FunctionDeclarationSyntax)scope.Symbol.Declarations.Single();
                        var functionName = Name(functionSyntax);
                        return new VariableName(functionName, parameter.Name.Value);
                    }
                case IdentifierNameSyntax identifierName:
                    {
                        var nameScope = LexicalScope(identifierName);
                        var symbol = nameScope.LookupName(identifierName.Name.Value);
                        if (symbol == null) // TODO should this be a compiler error?
                            return UnknownName.Instance;
                        var declaration = symbol.Declarations.Single();
                        return Name(declaration);
                    }
                case VariableDeclarationStatementSyntax variableDeclaration:
                    {
                        var functionName = Name(EnclosingFunction(variableDeclaration));
                        return new VariableName(functionName, variableDeclaration.Name.Value);
                    }
                case null:
                    return UnknownName.Instance;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public GlobalNamespaceName GlobalNamespaceName([NotNull] PackageSyntax s) => attributes.GetOrAdd(s, "GlobalNamespaceName", ComputeGlobalNamespaceName);

        [NotNull]
        private GlobalNamespaceName ComputeGlobalNamespaceName([NotNull] PackageSyntax package)
        {
            return new GlobalNamespaceName(Name(package));
        }
    }
}
