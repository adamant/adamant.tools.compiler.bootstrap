using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public partial class TreeCodeTemplate
    {
        private readonly Grammar grammar;

        public TreeCodeTemplate(Grammar grammar)
        {
            this.grammar = grammar;
        }

        private string TypeName(GrammarSymbol symbol)
        {
            return symbol.IsQuoted ? symbol.Text : $"{grammar.Prefix}{symbol.Text}{grammar.Suffix}";
        }

        private bool IsNew(GrammarRule rule, GrammarProperty property)
        {
            return BaseRules(rule)
                   .SelectMany(r => r.Properties.Where(p => p.Name == property.Name))
                   .Any();
        }

        private string TypeName(GrammarType type)
        {
            var value = TypeName(type.Symbol);
            if (type.IsOptional) value += "?";
            if (type.IsList) value = $"{grammar.ListType}<{type}>";
            return value;
        }

        private string BaseTypes(GrammarRule rule)
        {
            var parents = rule.Parents.Select(TypeName);
            if (!rule.Parents.Any())
                return "";

            return " : " + string.Join(", ", parents);
        }

        private IEnumerable<GrammarRule> BaseRules(GrammarRule rule)
        {
            var directParents = grammar.Rules.Where(r => rule.Parents.Contains(r.Nonterminal));
            return directParents.SelectMany(r => BaseRules(r).Append(r)).Distinct();
        }

        private FixedList<GrammarRule> ChildRules(GrammarRule rule)
        {
            return grammar.Rules.Where(r => r.Parents.Contains(rule.Nonterminal)).ToFixedList();
        }

        private string ClosedType(GrammarRule rule)
        {
            var children = ChildRules(rule);
            if (!children.Any()) return "";
            var builder = new StringBuilder();
            builder.AppendLine("    [Closed(");
            var lastChild = children[^1];
            foreach (var child in children)
            {
                builder.Append($"        typeof({TypeName(child.Nonterminal)})");
                builder.AppendLine(child == lastChild ? ")]" : ",");
            }
            return builder.ToString();
        }
    }
}
