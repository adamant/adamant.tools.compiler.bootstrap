using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string NameAttribute = "Name";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackageName Name(PackageSyntax s) => Name<PackageName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GlobalNamespaceName Name(CompilationUnitSyntax s) => Name<GlobalNamespaceName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionName Name(FunctionDeclarationSyntax s) => Name<FunctionName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VariableName Name(ParameterSyntax s) => Name<VariableName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VariableName Name(IdentifierNameSyntax s) => Name<VariableName>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Name Name(SyntaxBranchNode syntax)
        {
            return attributes.GetOrAdd(syntax, NameAttribute, ComputeName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TName Name<TName>(SyntaxBranchNode syntax)
            where TName : Name
        {
            return (TName)attributes.GetOrAdd(syntax, NameAttribute, ComputeName);
        }

        private Name ComputeName(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case PackageSyntax package:
                    // TODO get actual package name
                    return new PackageName("default");

                case CompilationUnitSyntax compilationUnit:
                    return GlobalNamespaceName(Parent(compilationUnit));

                case FunctionDeclarationSyntax functionDeclaration:
                    {
                        var parentName = Name(Parent(functionDeclaration));
                        return new FunctionName(parentName, functionDeclaration.Name.Value,
                            functionDeclaration.Parameters.Count());
                    }
                case ParameterSyntax parameter:
                    {
                        var scope = LexicalScope(parameter);
                        var functionSyntax = scope.Symbol.Declarations.Single();
                        var parentName = (ScopeName)Name(functionSyntax);
                        return new VariableName(parentName, parameter.Name.Text);
                    }
                case IdentifierNameSyntax identifierName:
                    {
                        var nameScope = LexicalScope(identifierName);
                        var symbol = nameScope.LookupName(identifierName.Name.Text);
                        var declaration = symbol.Declarations.Single();
                        return Name(declaration);
                    }
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GlobalNamespaceName GlobalNamespaceName(PackageSyntax s) => attributes.GetOrAdd(s, "GlobalNamespaceName", ComputeGlobalNamespaceName);

        private GlobalNamespaceName ComputeGlobalNamespaceName(PackageSyntax package)
        {
            return new GlobalNamespaceName(Name(package));
        }
    }
}
