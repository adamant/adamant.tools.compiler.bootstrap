using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class SyntaxSymbol
    {
        public bool IsPackage { get; }
        public string Name { get; }

        /// <summary>
        /// Variables can be redeclared, so for a variable that is declared multiple
        /// times within a function scope, each declaration is given a unique
        /// declaration number starting with 1. Symbols with a single declaration
        /// have a null declaration number. Note that partial class declarations
        /// are not redeclarations and are given a single symbol with multiple declarations
        /// </summary>
        public int? DeclarationNumber { get; }

        public IReadOnlyList<SyntaxBranchNode> Declarations { get; }

        public IReadOnlyList<SyntaxSymbol> Children { get; }

        protected SyntaxSymbol(
            bool isPackage,
            string name,
            int? declarationNumber,
            IEnumerable<SyntaxBranchNode> declarations,
            IEnumerable<SyntaxSymbol> children)
        {
            IsPackage = isPackage;
            Name = name;
            DeclarationNumber = declarationNumber;
            Declarations = declarations.ToList().AsReadOnly();
            Children = children.ToList().AsReadOnly();
        }

        public SyntaxSymbol(PackageSyntax declaration, IEnumerable<SyntaxSymbol> children)
            : this(true, "default", null, declaration.Yield(), children)
        {
        }

        public SyntaxSymbol(
            IEnumerable<CompilationUnitSyntax> declarations,
            IEnumerable<SyntaxSymbol> children)
            : this(false, "", null, declarations, children)
        {
        }
    }
}
