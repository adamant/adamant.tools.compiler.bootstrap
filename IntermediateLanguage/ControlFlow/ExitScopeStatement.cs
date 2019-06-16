using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// Marks the end of the scope of a variable that owns its value. Thus the
    /// value should not live beyond here.
    /// </summary>
    public class ExitScopeStatement : ExpressionStatement
    {
        public ExitScopeStatement(TextSpan span, Scope scope)
            : base(span, scope)
        {
        }

        public override Statement Clone()
        {
            return new ExitScopeStatement(Span, Scope);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"exit {Scope} // at {Span}";
        }
    }
}
