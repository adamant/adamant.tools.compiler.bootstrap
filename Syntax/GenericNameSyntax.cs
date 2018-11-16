using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericNameSyntax : SimpleNameSyntax
    {
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public GenericNameSyntax(
            TextSpan span,
            [NotNull] IIdentifierToken name,
            [NotNull] FixedList<ArgumentSyntax> arguments)
            : base(name, span)
        {
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Name}[{Arguments}]";
        }
    }
}
