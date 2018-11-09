using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class TypeDeclaration : Declaration
    {
        [CanBeNull, ItemNotNull] public IReadOnlyList<GenericParameter> GenericParameters { get; }
        public bool IsGeneric => GenericParameters != null;
        public int? GenericArity => GenericParameters?.Count;

        public TypeDeclaration(
            [NotNull] CodeFile file,
            [NotNull] Name name,
            [NotNull] DataType type,
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameter> genericParameters)
            : base(file, name, type)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(type), type);
            GenericParameters = genericParameters?.ToReadOnlyList();
        }
    }
}
