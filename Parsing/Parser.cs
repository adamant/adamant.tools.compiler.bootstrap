using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        protected CodeFile File { get; }
        protected ITokenIterator<IEssentialToken> Tokens { get; }
        private readonly RootName nameContext;
        private readonly VariableNumbers variableNumbers = new VariableNumbers();

        public Parser(ITokenIterator<IEssentialToken> tokens, RootName nameContext)
        {
            File = tokens.Context.File;
            Tokens = tokens;
            this.nameContext = nameContext;
        }

        protected void Add(Diagnostic diagnostic)
        {
            Tokens.Context.Diagnostics.Add(diagnostic);
        }

        /// <summary>
        /// A nested parser establishes a nested naming context for things parsed by it.
        /// </summary>
        protected Parser NestedParser(Name name)
        {
            return new Parser(Tokens, name);
        }
    }
}
