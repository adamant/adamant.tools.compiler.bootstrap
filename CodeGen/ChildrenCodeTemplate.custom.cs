using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public partial class ChildrenCodeTemplate
    {
        private readonly Grammar grammar;

        public ChildrenCodeTemplate(Grammar grammar)
        {
            this.grammar = grammar;
        }

        private string TypeName(GrammarSymbol symbol)
        {
            if (symbol.IsQuoted)
                return symbol.Text;

            // If it is a nonterminal, then transform the name
            if (grammar.Rules.Any(r => r.Nonterminal == symbol))
                return $"{grammar.Prefix}{symbol.Text}{grammar.Suffix}";

            return symbol.Text;
        }

        private bool IsLeaf(GrammarRule rule)
        {
            return !grammar.Rules.Any(r => r.Parents.Contains(rule.Nonterminal));
        }

        private bool IsNonTerminal(GrammarProperty property)
        {
            return grammar.Rules.Any(r => r.Nonterminal == property.Type);
        }

        //private bool IsNew(Rule rule, Property property)
        //{
        //    return BaseRules(rule)
        //           .SelectMany(r => r.Properties.Where(p => p.Name == property.Name))
        //           .Any();
        //}

        //private string TypeName(Property property)
        //{
        //    var type = TypeName(property.Type);
        //    if (property.IsOptional) type += "?";
        //    if (property.IsList) type = $"{grammar.ListType}<{type}>";
        //    return type;
        //}

        //private string BaseTypes(Rule rule)
        //{
        //    var parents = rule.Parents.Select(TypeName);
        //    if (!rule.Parents.Any())
        //        return "";

        //    return " : " + string.Join(", ", parents);
        //}

        //private IEnumerable<Rule> BaseRules(Rule rule)
        //{
        //    var directParents = grammar.Rules.Where(r => rule.Parents.Contains(r.Nonterminal));
        //    return directParents.SelectMany(r => BaseRules(r).Append(r)).Distinct();
        //}

        //private FixedList<Rule> ChildRules(Rule rule)
        //{
        //    return grammar.Rules.Where(r => r.Parents.Contains(rule.Nonterminal)).ToFixedList();
        //}

        //private string ClosedType(Rule rule)
        //{
        //    var children = ChildRules(rule);
        //    if (!children.Any()) return "";
        //    var builder = new StringBuilder();
        //    builder.AppendLine("    [Closed(");
        //    var lastChild = children[^1];
        //    foreach (var child in children)
        //    {
        //        builder.Append($"        typeof({TypeName(child.Nonterminal)})");
        //        builder.AppendLine(child == lastChild ? ")]" : ",");
        //    }
        //    return builder.ToString();
        //}
    }
}
