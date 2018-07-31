using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public struct SynthesizedAttributeRuleBuilder<T>
    {
        private readonly AttributeGrammar grammar;
        private readonly SynthesizedAttribute<T> attribute;

        public SynthesizedAttributeRuleBuilder(AttributeGrammar grammar, SynthesizedAttribute<T> attribute)
        {
            this.grammar = grammar;
            this.attribute = attribute;
        }

        public SynthesizedAttributeRuleBuilder<T> Value<TSyntax>(T value)
            where TSyntax : SyntaxBranchNode
        {
            var lazy = new Lazy<object>(value);
            grammar.Add<TSyntax>(attribute, (s, p) => lazy);
            return this;
        }

        public SynthesizedAttributeRuleBuilder<T> Rule<TSyntax>(Func<Attributes<TSyntax>, T> func)
            where TSyntax : SyntaxBranchNode
        {
            grammar.Add<TSyntax>(attribute, (s, c) => new Lazy<object>(func((Attributes<TSyntax>)s)));
            return this;
        }

        public SynthesizedAttributeRuleBuilder<T> Rule<TSyntax>(Func<Attributes<TSyntax>, IReadOnlyList<Attributes>, T> func)
            where TSyntax : SyntaxBranchNode
        {
            grammar.Add<TSyntax>(attribute, (s, c) => new Lazy<object>(func((Attributes<TSyntax>)s, c)));
            return this;
        }
    }
}
