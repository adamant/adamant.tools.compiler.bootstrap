using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class Package
    {
        [NotNull] public string Name { get; }
        public Diagnostics Diagnostics { get; internal set; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Declaration> Declarations { get; }
        [NotNull] [ItemNotNull] private readonly List<Declaration> declarations = new List<Declaration>();

        public Package([NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            Declarations = declarations.AsReadOnly().AssertNotNull();
        }

        internal void Add([NotNull] Declaration declaration)
        {
            Requires.NotNull(nameof(declaration), declaration);
            declarations.Add(declaration);
        }
    }
}
