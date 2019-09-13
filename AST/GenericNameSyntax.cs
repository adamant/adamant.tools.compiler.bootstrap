using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [VisitorNotSupported("Only implemented in parser")]
    public sealed class GenericNameSyntax : NameSyntax
    {
        public FixedList<ArgumentSyntax> Arguments { get; }
        public int Arity => Arguments.Count;

        public GenericNameSyntax(
            TextSpan span,
            string name,
            FixedList<ArgumentSyntax> arguments)
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
