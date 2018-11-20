using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Model;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class Package
    {
        [NotNull] public string Name { get; }
        [NotNull, ItemNotNull] public FixedList<Diagnostic> Diagnostics { get; internal set; }
        [NotNull] public FixedDictionary<string, Package> References { get; }
        [NotNull, ItemNotNull] public FixedList<Declaration> Declarations { get; }
        [CanBeNull] public FunctionDeclaration EntryPoint { get; internal set; }

        public Package(
            [NotNull] string name,
            [NotNull] FixedList<Diagnostic> diagnostics,
            [NotNull] IReadOnlyDictionary<string, Package> references,
            [NotNull] [ItemNotNull] IEnumerable<Declaration> declarations,
            [CanBeNull] FunctionDeclaration entryPoint)
        {
            Name = name;
            Diagnostics = diagnostics;
            References = new Dictionary<string, Package>(references).ToFixedDictionary();
            EntryPoint = entryPoint;
            Declarations = declarations.ToFixedList();
        }
    }
}
