namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser
    {
        private readonly ILexer lexer;

        public Parser(ILexer lexer)
        {
            this.lexer = lexer;
        }
    }
}
