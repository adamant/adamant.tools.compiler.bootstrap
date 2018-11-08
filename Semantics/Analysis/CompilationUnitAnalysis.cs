using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class CompilationUnitAnalysis : SyntaxAnalysis
    {
        [NotNull] public CompilationUnitScope GlobalScope { get; }
        [NotNull] public new CompilationUnitSyntax Syntax { get; }
        [NotNull] public NamespaceDeclarationAnalysis Namespace { get; }
        [NotNull] public IReadOnlyList<MemberDeclarationAnalysis> MemberDeclarations { get; }

        public CompilationUnitAnalysis(
            [NotNull] CompilationUnitScope globalScope,
            [NotNull] CompilationUnitSyntax syntax,
            [NotNull] NamespaceDeclarationAnalysis @namespace)
            : base(new AnalysisContext(syntax.CodeFile, globalScope), syntax)
        {
            Requires.NotNull(nameof(globalScope), globalScope);
            Requires.NotNull(nameof(@namespace), @namespace);
            GlobalScope = globalScope;
            Syntax = syntax;
            Namespace = @namespace;
            MemberDeclarations = GatherMemberDeclarations(@namespace).ToReadOnlyList();
        }

        private static List<MemberDeclarationAnalysis> GatherMemberDeclarations(NamespaceDeclarationAnalysis namespaceDeclaration)
        {
            var memberDeclarations = new List<MemberDeclarationAnalysis>();
            var namespaces = new Queue<NamespaceDeclarationAnalysis>();
            namespaces.Enqueue(namespaceDeclaration);
            while (namespaces.TryDequeue(out namespaceDeclaration))
                foreach (var declaration in namespaceDeclaration.Declarations)
                    switch (declaration)
                    {
                        case NamespaceDeclarationAnalysis nestedNamespace:
                            namespaces.Enqueue(nestedNamespace);
                            break;
                        case MemberDeclarationAnalysis memberDeclaration:
                            memberDeclarations.Add(memberDeclaration);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(declaration);
                    }

            return memberDeclarations;
        }
    }
}
