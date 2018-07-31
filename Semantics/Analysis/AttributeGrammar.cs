using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public delegate Lazy<object> InheritedAttributeRule(Attributes node, Attributes parent);
    public delegate Lazy<object> SynthesizedAttributeRule(Attributes node, IReadOnlyList<Attributes> children);

    public class AttributeGrammar
    {
        private readonly AttributeRules<InheritedAttribute, InheritedAttributeRule> inheritedAttributeRules = new AttributeRules<InheritedAttribute, InheritedAttributeRule>();
        private readonly AttributeRules<SynthesizedAttribute, SynthesizedAttributeRule> synthesizedAttributeRules = new AttributeRules<SynthesizedAttribute, SynthesizedAttributeRule>();

        public InheritedAttributeRuleBuilder<T> For<T>(InheritedAttribute<T> attribute)
        {
            return new InheritedAttributeRuleBuilder<T>(this, attribute);
        }

        public SynthesizedAttributeRuleBuilder<T> For<T>(SynthesizedAttribute<T> attribute)
        {
            return new SynthesizedAttributeRuleBuilder<T>(this, attribute);
        }

        public void Add<TSyntax>(InheritedAttribute attribute, InheritedAttributeRule rule)
        {
            inheritedAttributeRules.Add<TSyntax>(attribute, rule);
        }

        public void Add<TSyntax>(SynthesizedAttribute attribute, SynthesizedAttributeRule rule)
        {
            synthesizedAttributeRules.Add<TSyntax>(attribute, rule);
        }

        public Attributes<PackageSyntax> ApplyTo(PackageSyntax package)
        {
            throw new NotImplementedException();
        }
    }
}
