using System;
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
        public CompilationUnitSyntax Parent(ClassDeclarationSyntax s) => Parent<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompilationUnitSyntax Parent(EnumStructDeclarationSyntax s) => Parent<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SyntaxNode Parent(SyntaxNode s)
        {
            return attributes.Get<SyntaxNode>(s, ParentAttribute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TSyntax Parent<TSyntax>(SyntaxNode syntax)
            where TSyntax : SyntaxNode
        {
            return attributes.Get<TSyntax>(syntax, ParentAttribute);
        }

        private void AddParentAttributes(PackageSyntax package)
        {
            AddParentAttributes(package, null);
        }

        private void AddParentAttributes(SyntaxNode syntax, SyntaxNode parent)
        {
            //attributes.GetOrAdd<SyntaxNode>(syntax, ParentAttribute, new Lazy<object>(parent));
            //foreach (var child in syntax.ChildNodes.OfType<SyntaxNode>())
            //    AddParentAttributes(child, syntax);
            throw new NotImplementedException();
        }
    }
}
