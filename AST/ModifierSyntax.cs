using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ModifierSyntax : Syntax
    {
        public readonly AccessModifier Access;

        public ModifierSyntax(TextSpan span, AccessModifier access)
            : base(span)
        {
            Access = access;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
