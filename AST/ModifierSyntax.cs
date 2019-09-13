namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class ModifierSyntax : Syntax
    {
        public readonly AccessModifier Access;

        public ModifierSyntax(AccessModifier access)
        {
            Access = access;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
