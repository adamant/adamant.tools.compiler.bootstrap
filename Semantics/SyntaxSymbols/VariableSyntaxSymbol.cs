using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class VariableSyntaxSymbol : ISyntaxSymbol
    {
        public string Name { get; }
        public int? DeclarationNumber { get; }

        public SyntaxNode Declaration { get; }
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => Declaration.Yield();

        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => Enumerable.Empty<ISyntaxSymbol>();

        public VariableSyntaxSymbol(ParameterSyntax declaration, int? declarationNumber)
        {
            Name = declaration.Name.Value;
            DeclarationNumber = declarationNumber;
            Declaration = declaration;
        }

        public VariableSyntaxSymbol(VariableDeclarationStatementSyntax declaration, int? declarationNumber)
        {
            Name = declaration.Name.Value;
            DeclarationNumber = declarationNumber;
            Declaration = declaration;
        }
    }
}
