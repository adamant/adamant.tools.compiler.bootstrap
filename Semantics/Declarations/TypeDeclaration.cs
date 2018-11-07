using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class TypeDeclaration : Declaration
    {
        [NotNull, ItemNotNull] public IReadOnlyList<GenericParameter> GenericParameters { get; }
        public int GenericArity => GenericParameters.Count;

        public TypeDeclaration(
            [NotNull] CodeFile file,
            [NotNull] Name name,
            [NotNull] KnownType type,
            [NotNull, ItemNotNull] IEnumerable<GenericParameter> genericParameters)
            : base(file, name, type)
        {
            Requires.NotNull(nameof(genericParameters), genericParameters);
            GenericParameters = genericParameters.ToReadOnlyList();
        }
    }
}
