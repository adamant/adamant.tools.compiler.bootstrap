using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class DeclarationSyntax : Syntax, IDeclarationSyntax
    {
        public CodeFile File { get; }
        public TextSpan NameSpan { get; }

        protected DeclarationSyntax(TextSpan span, CodeFile file, TextSpan nameSpan)
            : base(span)
        {
            NameSpan = nameSpan;
            File = file;
        }
    }
}
