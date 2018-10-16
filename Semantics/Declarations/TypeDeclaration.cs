using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class TypeDeclaration : Declaration
    {
        public TypeDeclaration([NotNull] CodeFile file, [NotNull] QualifiedName qualifiedName)
            : base(file, qualifiedName)
        {
        }
    }
}
