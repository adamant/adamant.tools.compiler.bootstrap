using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class ILPackage
    {
        [NotNull] public readonly string Name;
        [NotNull] [ItemNotNull] public IReadOnlyList<ILDeclaration> Declarations { get; }
        [NotNull] [ItemNotNull] private readonly List<ILDeclaration> declarations = new List<ILDeclaration>();

        public ILPackage([NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            Declarations = declarations.AsReadOnly().AssertNotNull();
        }

        public void Add([NotNull] ILDeclaration declaration)
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
