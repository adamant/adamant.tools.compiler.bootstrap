using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class Package
    {
        public FixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }

        public Package(FixedSet<INonMemberDeclaration> nonMemberDeclarations)
        {
            NonMemberDeclarations = nonMemberDeclarations;
        }
    }
}
