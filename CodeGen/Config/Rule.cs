using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class Rule
    {
        public Symbol Nonterminal { get; }
        public FixedList<Symbol> Parents { get; }

        public FixedList<Property> Properties { get; }

        public Rule(
            Symbol nonterminal,
            IEnumerable<Symbol> parents,
            IEnumerable<Property> properties)
        {
            Nonterminal = nonterminal;
            Parents = parents.ToFixedList();
            Properties = properties.ToFixedList();
        }
    }
}
