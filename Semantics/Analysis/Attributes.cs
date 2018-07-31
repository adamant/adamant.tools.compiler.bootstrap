using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class Attributes
    {
        public SyntaxBranchNode Syntax => GetSyntax();

        private readonly SortedDictionary<InheritedAttribute, Lazy<object>> inheritedAttributes = new SortedDictionary<InheritedAttribute, Lazy<object>>();
        private readonly SortedDictionary<SynthesizedAttribute, Lazy<object>> synthesizedAttributes = new SortedDictionary<SynthesizedAttribute, Lazy<object>>();

        protected abstract SyntaxBranchNode GetSyntax();

        public T Get<T>(InheritedAttribute<T> attribute)
        {
            if (inheritedAttributes.TryGetValue(attribute, out var lazy))
                return (T)lazy.Value;

            throw new Exception($"Attribute {attribute.Name} is not defined for {Syntax.GetType().Name}.");
        }


        public T Get<T>(SynthesizedAttribute<T> attribute)
        {
            if (synthesizedAttributes.TryGetValue(attribute, out var lazy))
                return (T)lazy.Value;

            throw new Exception($"Attribute {attribute.Name} is not defined for {Syntax.GetType().Name}.");
        }

        public Type SemanticNodeType() => Get(Attribute.SemanticNodeType);

        public DataType Type() => Get(Attribute.Type);

        public SemanticNode SemanticNode() => Get(Attribute.SemanticNode);
    }

    public class Attributes<TSyntax> : Attributes
        where TSyntax : SyntaxBranchNode
    {
        public new TSyntax Syntax { get; }

        public Attributes(TSyntax syntax)
        {
            Syntax = syntax;
        }

        protected override SyntaxBranchNode GetSyntax()
        {
            return Syntax;
        }
    }
}
