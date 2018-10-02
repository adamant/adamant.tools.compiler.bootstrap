using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxBranchNode
    {
        public CompilationUnitNamespaceSyntax Namespace { get; }
        public IReadOnlyList<UsingDirectiveSyntax> UsingDirectives { get; }
        public IReadOnlyList<DeclarationSyntax> Declarations { get; }
        public Token EndOfFile { get; }

        public CompilationUnitSyntax(
            CompilationUnitNamespaceSyntax @namespace,
            IReadOnlyList<UsingDirectiveSyntax> usingDirectives,
            IReadOnlyList<DeclarationSyntax> declarations,
            Token endOfFile)
            : base(@namespace.YieldValue<SyntaxNode>()
                .Concat(usingDirectives)
                .Concat(declarations)
                .Append(endOfFile))
        {
            Namespace = @namespace;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            EndOfFile = endOfFile;
        }
    }
}
