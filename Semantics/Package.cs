using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class Package
    {
        [NotNull] public string Name { get; }
        [NotNull] [ItemNotNull] public Diagnostics Diagnostics { get; internal set; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Namespace> Namespaces { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Declaration> Declarations { get; }
        [CanBeNull] public FunctionDeclaration EntryPoint { get; internal set; }

        public Package(
            [NotNull] string name,
            [NotNull] Diagnostics diagnostics,
            [NotNull][ItemNotNull] IEnumerable<Namespace> namespaces,
            [NotNull] [ItemNotNull]  IEnumerable<Declaration> declarations,
            [CanBeNull] FunctionDeclaration entryPoint)
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Requires.NotNull(nameof(declarations), declarations);
            Name = name;
            Diagnostics = diagnostics;
            EntryPoint = entryPoint;
            Namespaces = namespaces.ToReadOnlyList();
            Declarations = declarations.ToReadOnlyList();
        }
    }
}
