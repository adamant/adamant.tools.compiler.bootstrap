using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public partial class CodeTemplate
    {
        private readonly Grammar grammar;
        private readonly FixedList<string> baseType;

        public CodeTemplate(Grammar grammar)
        {
            this.grammar = grammar;
            baseType = grammar.BaseType.YieldValue().ToFixedList();
        }

        private string TypeName(string name)
        {
            // If it is a nonterminal, then transform the name
            if (grammar.Rules.Any(r => r.Nonterminal == name))
                return $"{grammar.Prefix}{name}{grammar.Suffix}";

            return name;
        }

        private bool IsNew(Rule rule, Property property)
        {
            return BaseRules(rule)
                   .SelectMany(r => r.Properties.Where(p => p.Name == property.Name))
                   .Any();
        }

        private string TypeName(Property property)
        {
            var type = TypeName(property.Type);
            if (property.IsOptional) type += "?";
            if (property.IsList) type = $"{grammar.ListType}<{type}>";
            return type;
        }

        private string BaseTypes(Rule rule)
        {
            var parents = rule.Parents.Select(TypeName);
            if (!rule.Parents.Any())
            {
                if (grammar.BaseType is null) return "";
                parents = baseType;
            }

            return " : " + string.Join(", ", parents);
        }

        private IEnumerable<Rule> BaseRules(Rule rule)
        {
            var directParents = grammar.Rules.Where(r => rule.Parents.Contains(r.Nonterminal));
            return directParents.SelectMany(r => BaseRules(r).Append(r)).Distinct();
        }
    }
}
