using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class AttributeRules<TAttribute, TRule>
    {
        private readonly Dictionary<TAttribute, Dictionary<Type, TRule>> rules = new Dictionary<TAttribute, Dictionary<Type, TRule>>();

        public void Add<TSyntax>(TAttribute attribute, TRule rule)
        {
            if (!rules.TryGetValue(attribute, out var rulesByType))
            {
                rulesByType = new Dictionary<Type, TRule>();
                rules.Add(attribute, rulesByType);
            }
            rulesByType.Add(typeof(TSyntax), rule);
        }
    }
}
