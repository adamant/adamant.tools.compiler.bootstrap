using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class TypeDeclaration : Declaration
    {
        public TypeDeclaration(
            [NotNull] CodeFile file,
            [NotNull] QualifiedName name,
            [NotNull] KnownType type)
            : base(file, name, type)
        {
        }
    }
}
