using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class CompilationUnitAnalysis : SyntaxAnalysis
    {
        [NotNull] public CompilationUnitScope GlobalScope { get; }
        [NotNull] public CompilationUnitSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public FixedList<DeclarationAnalysis> Declarations { get; }
        [NotNull] public FixedList<MemberDeclarationAnalysis> MemberDeclarations { get; }

        public CompilationUnitAnalysis(
            [NotNull] CompilationUnitScope globalScope,
            [NotNull] CompilationUnitSyntax syntax,
            [NotNull, ItemNotNull] FixedList<DeclarationAnalysis> declarations)
            : base(new AnalysisContext(syntax.CodeFile, globalScope))
        {
            GlobalScope = globalScope;
            Syntax = syntax;
            Declarations = declarations;
            MemberDeclarations = GatherMemberDeclarations(declarations).ToFixedList();
        }

        [NotNull]
        private static List<MemberDeclarationAnalysis> GatherMemberDeclarations(
            [NotNull, ItemNotNull] FixedList<DeclarationAnalysis> declarations)
        {
            var memberDeclarations = new List<MemberDeclarationAnalysis>();
            memberDeclarations.AddRange(declarations.OfType<MemberDeclarationAnalysis>());
            var namespaces = new Queue<NamespaceDeclarationAnalysis>();
            namespaces.EnqueueRange(declarations.OfType<NamespaceDeclarationAnalysis>());
            while (namespaces.TryDequeue(out var namespaceDeclaration))
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
