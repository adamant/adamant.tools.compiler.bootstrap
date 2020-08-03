using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class Rule
    {
        public GrammarSymbol Nonterminal { get; }
        public FixedList<GrammarSymbol> Parents { get; }

        public FixedList<Property> Properties { get; }

        public Rule(
            GrammarSymbol nonterminal,
            IEnumerable<GrammarSymbol> parents,
            IEnumerable<Property> properties)
        {
            Nonterminal = nonterminal;
            Parents = parents.ToFixedList();
            Properties = properties.ToFixedList();
        }
    }
}
