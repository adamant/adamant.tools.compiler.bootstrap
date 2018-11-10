namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ModifierSyntax : NonTerminal
    {
        public readonly AccessModifier Access;


        public ModifierSyntax(AccessModifier access)
        {
            Access = access;
        }
    }
}
