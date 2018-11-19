using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
