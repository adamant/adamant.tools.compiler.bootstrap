using System;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser : RecursiveDescentParser
    {
        private readonly NamespaceName? containingNamespace;
        private NamespaceName ContainingNamespace => containingNamespace
            ?? throw new InvalidOperationException("Non-member declaration not nested inside a containing namespace");

        public Parser(ITokenIterator<IEssentialToken> tokens, NamespaceName? containingNamespace)
            : base(tokens)
        {
            this.containingNamespace = containingNamespace;
        }

        /// <summary>
        /// A member parser drops the containingNamespace because there isn't one
        /// </summary>
        protected Parser BodyParser()
        {
            return containingNamespace is null ? this : new Parser(Tokens, null);
        }

        /// <summary>
        /// A nested parser establishes a nested naming context for things parsed by it.
        /// </summary>
        protected Parser NamespaceBodyParser(NamespaceName namespaceName)
        {
            _ = containingNamespace
                ?? throw new InvalidOperationException("Namespace not nested inside a containing namespace");
            return new Parser(Tokens, containingNamespace.Qualify(namespaceName));
        }
    }
}
