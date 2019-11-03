using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax, IDeclarationSyntax
    {
        bool IsGlobalQualified { get; }

        /// <summary>
        /// The name as declared with this declaration
        /// </summary>
        Name Name { get; }

        /// <summary>
        /// The name including any namespaces this declaration is nested in
        /// </summary>
        Name FullName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
    }
}
