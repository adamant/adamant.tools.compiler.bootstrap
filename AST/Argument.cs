namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// This is not considered a piece of syntax, this is simply a container that
    /// allows us to take a reference to the expression passed as an argument.
    /// </summary>
    public class Argument
    {
        private IExpressionSyntax value;
        public ref IExpressionSyntax Value => ref value;
        public Argument(IExpressionSyntax value)
        {
            this.value = value;
        }
    }
}
