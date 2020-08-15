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
            return grammar.Rules.Any(r => r.Nonterminal == property.Type.Symbol);
        }
    }
}
