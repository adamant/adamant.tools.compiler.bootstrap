using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GenericNameSyntax : SimpleNameSyntax
    {
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }
        public int Arity => Arguments.Count;

        [NotNull] public TypePromise NameType { get; } = new TypePromise();

        public GenericNameSyntax(
            TextSpan span,
            [NotNull] string name,
            [NotNull] FixedList<ArgumentSyntax> arguments)
            : base(new SimpleName(name), span)
        {
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Name}[{Arguments}]";
        }
    }
}
