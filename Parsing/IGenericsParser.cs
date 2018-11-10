using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public interface IGenericsParser
    {
        [MustUseReturnValue]
        [CanBeNull]
        GenericParametersSyntax AcceptGenericParameters(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics);

        [MustUseReturnValue]
        [NotNull]
        FixedList<GenericConstraintSyntax> ParseGenericConstraints(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics);
    }
}
