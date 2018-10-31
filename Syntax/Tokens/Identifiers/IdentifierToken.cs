using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers
{
    public interface IIdentifierToken : IIdentifierOrPrimitiveToken, ILifetimeNameToken
    {
        [CanBeNull] string Value { get; }
    }

    public abstract class IdentifierToken : Token, IIdentifierToken
    {
        [NotNull] public string Value { get; }

        protected IdentifierToken(TextSpan span, [NotNull] string value)
            : base(span)
        {
            Requires.NotNull(nameof(value), value);
            Value = value;
        }
    }
}
