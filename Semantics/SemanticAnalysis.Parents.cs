using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string ParentAttribute = "Parent";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackageSyntax Parent(CompilationUnitSyntax s) => Parent<PackageSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompilationUnitSyntax Parent(FunctionDeclarationSyntax s) => Parent<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SyntaxBranchNode Parent(SyntaxBranchNode s)
        {
            return attributes.Get<SyntaxBranchNode>(s, ParentAttribute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TSyntax Parent<TSyntax>(SyntaxBranchNode syntax)
            where TSyntax : SyntaxBranchNode
        {
            return attributes.Get<TSyntax>(syntax, ParentAttribute);
        }

        private void AddParentAttributes(PackageSyntax package)
        {
            AddParentAttributes(package, null);
        }

        private void AddParentAttributes(SyntaxBranchNode syntax, SyntaxBranchNode parent)
        {
            attributes.GetOrAdd<SyntaxBranchNode>(syntax, ParentAttribute, new Lazy<object>(parent));
            foreach (var child in syntax.Children.OfType<SyntaxBranchNode>())
                AddParentAttributes(child, syntax);
        }
    }
}
