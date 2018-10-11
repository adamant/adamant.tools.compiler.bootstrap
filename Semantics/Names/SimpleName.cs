using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class SimpleName : Name
    {
        [NotNull] public readonly string Text;
        [NotNull] public readonly bool IsSpecial;

        public SimpleName([NotNull] string text)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            IsSpecial = false;
        }

        public SimpleName([NotNull] string text, bool isSpecial)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            IsSpecial = isSpecial;
        }

        public static implicit operator SimpleName([NotNull] string text)
        {
            return new SimpleName(text);
        }
    }
}
