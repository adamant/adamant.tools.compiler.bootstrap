using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public struct InheritedAttributeRuleBuilder<T>
    {
        private readonly AttributeGrammar grammar;
        private readonly InheritedAttribute<T> attribute;

        public InheritedAttributeRuleBuilder(AttributeGrammar grammar, InheritedAttribute<T> attribute)
        {
            this.grammar = grammar;
            this.attribute = attribute;
        }

        public InheritedAttributeRuleBuilder<T> Value<TSyntax>(T value)
            where TSyntax : SyntaxBranchNode
        {
            var lazy = new Lazy<object>(value);
            grammar.Add<TSyntax>(attribute, (s, p) => lazy);
            return this;
        }
    }
}
