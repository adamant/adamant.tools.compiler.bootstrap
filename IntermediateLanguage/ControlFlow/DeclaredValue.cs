using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// Value used for names like function names where they are just declared in the
    /// global namespace
    /// </summary>
    public class DeclaredValue : Value
    {
        [NotNull] public Name Name { get; }

        public DeclaredValue([NotNull] Name name)
        {
            Name = name;
        }

        [NotNull]
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
