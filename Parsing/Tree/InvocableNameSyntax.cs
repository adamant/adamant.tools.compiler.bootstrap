using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class InvocableNameSyntax : Syntax, IInvocableNameSyntax
    {
        public MaybeQualifiedName Name { get; }

        public InvocableNameSyntax(TextSpan span, SimpleName name)
            : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
