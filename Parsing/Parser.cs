using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        protected readonly CodeFile File;
        protected readonly ITokenIterator Tokens;
        private readonly RootName nameContext;
        private readonly VariableNumbers variableNumbers = new VariableNumbers();

        public Parser(ITokenIterator tokens, RootName nameContext)
        {
            File = tokens.Context.File;
            Tokens = tokens;
            this.nameContext = nameContext;
        }

        protected void Add(Diagnostic diagnostic)
        {
            Tokens.Context.Diagnostics.Add(diagnostic);
        }

        protected Parser NestedParser(Name name)
        {
            return new Parser(Tokens, name);
        }
    }
}
