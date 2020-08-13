using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax
{
    internal class InvocableNameSyntax : IInvocableNameSyntax
    {
        public TextSpan Span { get; }
        public MaybeQualifiedName Name { get; }

        public InvocableNameSyntax(TextSpan span, MaybeQualifiedName name)
        {
            Span = span;
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
