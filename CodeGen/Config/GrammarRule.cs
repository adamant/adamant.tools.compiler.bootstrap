using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class GrammarRule
    {
        public GrammarSymbol Nonterminal { get; }
        public FixedList<GrammarSymbol> Parents { get; }

        public FixedList<GrammarProperty> Properties { get; }

        public GrammarRule(
            GrammarSymbol nonterminal,
            IEnumerable<GrammarSymbol> parents,
            IEnumerable<GrammarProperty> properties)
        {
            Nonterminal = nonterminal;
            Parents = parents.ToFixedList();
            Properties = properties.ToFixedList();
        }
    }
}
