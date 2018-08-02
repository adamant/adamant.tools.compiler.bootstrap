using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class ParentAttribute : SemanticAttribute
    {
        public const string Key = "Parent";
        public override string AttributeKey => Key;

        public ParentAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public PackageSyntax this[CompilationUnitSyntax s] => Get<PackageSyntax>(s);
        public CompilationUnitSyntax this[FunctionDeclarationSyntax s] => Get<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SyntaxBranchNode Get(SyntaxBranchNode syntax)
        {
            return Attributes.Get<SyntaxBranchNode>(syntax, Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TSyntax Get<TSyntax>(SyntaxBranchNode syntax)
            where TSyntax : SyntaxBranchNode
        {
            return Attributes.Get<TSyntax>(syntax, Key);
        }
    }
}
