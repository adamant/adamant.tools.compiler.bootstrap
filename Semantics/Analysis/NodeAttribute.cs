using System;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class NodeAttribute : SemanticAttribute
    {
        public const string Key = "Node";
        public override string AttributeKey => Key;

        public NodeAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public Package this[PackageSyntax s] => Get<Package>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SemanticNode Get(SyntaxBranchNode syntax)
        {
            return Attributes.Get(syntax, Key, Compute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TNode Get<TNode>(SyntaxBranchNode syntax)
            where TNode : SemanticNode
        {
            return (TNode)Attributes.Get(syntax, Key, Compute);
        }

        private SemanticNode Compute(SyntaxBranchNode syntax)
        {
            throw new NotImplementedException();
        }
    }
}
