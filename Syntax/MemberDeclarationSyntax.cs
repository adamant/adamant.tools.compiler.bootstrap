using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class MemberDeclarationSyntax : DeclarationSyntax
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

        protected MemberDeclarationSyntax(TextSpan nameSpan)
        {
            NameSpan = nameSpan;
        }
    }
}
