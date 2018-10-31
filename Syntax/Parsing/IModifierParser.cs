using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public interface IModifierParser
    {
        [CanBeNull]
        ModifierSyntax AcceptModifier(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics);
    }
}
