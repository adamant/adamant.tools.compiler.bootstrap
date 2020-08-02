using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(INamespaceDeclarationSyntax),
        typeof(IEntityDeclarationSyntax))]
    public partial interface IDeclarationSyntax : ISyntax
    {
        CodeFile File { get; }

        /// <summary>
        /// The span of whatever would count as the "name" of this declaration
        /// for things like operator overloads, constructors and destructors,
        /// this won't be just an identifier. For example, it could be:
        /// * "operator +"
        /// * "new foo"
        /// * "delete"
        /// </summary>
        TextSpan NameSpan { get; }
    }
}
