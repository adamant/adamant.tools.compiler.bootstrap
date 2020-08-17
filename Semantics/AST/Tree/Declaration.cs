using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class Declaration : AbstractSyntax, IDeclaration
    {
        public CodeFile File { get; }
        public Symbol Symbol { get; }
        public TextSpan NameSpan { get; }

        protected Declaration(CodeFile file, TextSpan span, Symbol symbol, TextSpan nameSpan)
            : base(span)
        {
            Symbol = symbol;
            NameSpan = nameSpan;
            File = file;
        }
    }
}
