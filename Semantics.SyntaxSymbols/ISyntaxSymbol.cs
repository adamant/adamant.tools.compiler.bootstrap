using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public interface ISyntaxSymbol
    {
        string Name { get; }

        /// <summary>
        /// Variables can be redeclared, so for a variable that is declared multiple
        /// times within a function scope, each declaration is given a unique
        /// declaration number starting with 1. Symbols with a single declaration
        /// have a null declaration number. Note that partial class declarations
        /// are not redeclarations and are given a single symbol with multiple declarations
        /// </summary>
        int? DeclarationNumber { get; }

        IReadOnlyList<ISyntaxSymbol> Children { get; }

        IReadOnlyList<SyntaxBranchNode> Declarations { get; }
    }
}
