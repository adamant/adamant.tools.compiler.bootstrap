using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class Rule
    {
        public string Nonterminal { get; }
        public FixedList<string> Parents { get; }

        public FixedList<Property> Properties { get; }

        public Rule(
            string nonterminal,
            IEnumerable<string> parents,
            IEnumerable<Property> properties)
        {
            Nonterminal = nonterminal;
            Parents = parents.ToFixedList();
            Properties = properties.ToFixedList();
        }
    }
}
