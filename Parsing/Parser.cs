using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser : RecursiveDescentParser
    {
        private readonly RootName nameContext;
        private readonly NamespaceName? containingNamespace;
        private readonly VariableNumbers variableNumbers = new VariableNumbers();

        public Parser(ITokenIterator<IEssentialToken> tokens, RootName nameContext, NamespaceName? containingNamespace)
            : base(tokens)
        {
            this.nameContext = nameContext;
            this.containingNamespace = containingNamespace;
        }

        /// <summary>
        /// A nested parser establishes a nested naming context for things parsed by it.
        /// </summary>
        protected Parser NestedParser(RootName nameContext, NamespaceName? containingNamespace)
        {
            return new Parser(Tokens, nameContext, containingNamespace);
        }
    }
}
