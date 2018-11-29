using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Model
{
    public class TypeDeclaration : Declaration
    {
        [CanBeNull, ItemNotNull] public FixedList<GenericParameter> GenericParameters { get; }
        public bool IsGeneric => GenericParameters != null;
        public int? GenericArity => GenericParameters?.Count;

        public TypeDeclaration(
            [NotNull] Name name,
            [NotNull] DataType type,
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameter> genericParameters,
            [NotNull] FixedList<Declaration> members)
            : base(name, type, members.ToFixedDictionary(m => m.FullName.UnqualifiedName, m => (ISymbol)m))
        {
            GenericParameters = genericParameters?.ToFixedList();
        }
    }
}
