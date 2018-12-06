using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class Package
    {
        public string Name { get; }
        public FixedList<Diagnostic> Diagnostics { get; internal set; }
        public FixedDictionary<string, Package> References { get; }
        public FixedList<Declaration> Declarations { get; }
        public FunctionDeclaration EntryPoint { get; internal set; }

        public Package(
            string name,
            FixedList<Diagnostic> diagnostics,
            IReadOnlyDictionary<string, Package> references,
            IEnumerable<Declaration> declarations,
            FunctionDeclaration entryPoint)
        {
            Name = name;
            Diagnostics = diagnostics;
            References = new Dictionary<string, Package>(references).ToFixedDictionary();
            EntryPoint = entryPoint;
            Declarations = declarations.ToFixedList();
        }
    }
}
