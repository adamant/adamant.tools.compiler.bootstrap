using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class DeclarationSyntax : Syntax
    {
        /// <summary>
        /// The span of whatever would count as the "name" of this declaration
        /// for things like operator overloads, constructors and destructors,
        /// this won't be just an identifier. For example, it could be:
        /// * "operator +"
        /// * "new foo"
        /// * "delete"
        /// </summary>
        public TextSpan NameSpan { get; }

        protected DeclarationSyntax(TextSpan nameSpan)
        {
            NameSpan = nameSpan;
        }
    }
}
