using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
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
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameter> genericParameters)
            : base(name, type)
        {
            GenericParameters = genericParameters?.ToFixedList();
        }

        [CanBeNull]
        public override ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
