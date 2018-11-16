using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IGenericsParser
    {
        [MustUseReturnValue]
        [CanBeNull]
        FixedList<GenericParameterSyntax> AcceptGenericParameters();

        [MustUseReturnValue]
        [NotNull]
        FixedList<GenericConstraintSyntax> ParseGenericConstraints();
    }
}
