using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Declarations
{
    public class TypeDeclarationIL : DeclarationIL
    {
        [NotNull] public readonly string Name;
        public readonly bool IsReference;

        public TypeDeclarationIL([NotNull]string name, bool isReference)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            IsReference = isReference;
        }

        internal override void ToString([NotNull] AsmBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            var kind = IsReference ? "class" : "struct";
            builder.AppendLine($"public {kind} {Name}");
            builder.BeginBlock();
            builder.EndBlock();
        }
    }
}
