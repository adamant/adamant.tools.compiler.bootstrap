using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class InvocableNameSyntax : Syntax, IInvocableNameSyntax
    {
        public Name Name { get; }

        public InvocableNameSyntax(TextSpan span, Name name)
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
