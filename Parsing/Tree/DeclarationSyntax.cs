using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class DeclarationSyntax : Syntax, IDeclarationSyntax
    {
        public CodeFile File { get; }
        public bool HasErrors { get; private set; }

        /// <summary>
        /// The span of whatever would count as the "name" of this declaration
        /// for things like operator overloads, constructors and destructors,
        /// this won't be just an identifier. For example, it could be:
        /// * "operator +"
        /// * "new foo"
        /// * "delete"
        /// </summary>
        public TextSpan NameSpan { get; }

        protected DeclarationSyntax(TextSpan span, CodeFile file, TextSpan nameSpan)
            : base(span)
        {
            NameSpan = nameSpan;
            File = file;
        }

        public void MarkErrored()
        {
            HasErrors = true;
        }
    }
}