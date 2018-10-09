using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class PackageIL
    {
        [NotNull] public readonly string Name;
        [NotNull] [ItemNotNull] public IReadOnlyList<DeclarationIL> Declarations { get; }
        [NotNull] [ItemNotNull] private readonly List<DeclarationIL> declarations = new List<DeclarationIL>();

        public PackageIL([NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            Declarations = declarations.AsReadOnly().AssertNotNull();
        }

        public void Add([NotNull] DeclarationIL declaration)
        {
            Requires.NotNull(nameof(declaration), declaration);
            declarations.Add(declaration);
        }

        public override string ToString()
        {
            var builder = new AsmBuilder();
            builder.AppendLine($"package {Name}");
            foreach (var declaration in declarations)
            {
                builder.BlankLine(); // This will give us a blank line after the package declaration too
                declaration.ToString(builder);
            }

            return builder.Code;
        }
    }
}
