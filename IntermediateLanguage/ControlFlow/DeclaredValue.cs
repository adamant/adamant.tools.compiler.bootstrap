using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// Value used for names like function names where they are just declared in the
    /// global namespace
    /// </summary>
    public class DeclaredValue : Value
    {
        public Name Name { get; }

        public DeclaredValue(Name name, TextSpan span)
           : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
