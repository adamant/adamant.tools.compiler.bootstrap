using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class Grammar
    {
        public string? Namespace { get; }
        public Symbol? BaseType { get; }
        public string Prefix { get; }
        public string Suffix { get; }
        public string ListType { get; }
        public FixedList<string> UsingNamespaces { get; }
        public FixedList<Rule> Rules { get; }

        public Grammar(
            string? @namespace,
            Symbol? baseType,
            string prefix,
            string suffix,
            string listType,
            IEnumerable<string> usingNamespaces,
            IEnumerable<Rule> rules)
        {
            Namespace = @namespace;
            BaseType = baseType;
            Prefix = prefix;
            Suffix = suffix;
            ListType = listType;
            UsingNamespaces = usingNamespaces.ToFixedList();
            Rules = rules.ToFixedList();
        }
    }
}
