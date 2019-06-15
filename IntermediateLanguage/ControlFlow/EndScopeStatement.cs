using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// Marks the end of the scope of a variable that owns its value. Thus the
    /// value should not live beyond here.
    /// </summary>
    public class EndScopeStatement : ExpressionStatement
    {
        public Place Place { get; }

        public EndScopeStatement(Place place, TextSpan span)
            : base(span)
        {
            Place = place;
        }

        public override Statement Clone()
        {
            return new EndScopeStatement(Place, Span);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"end_scope {Place} // at {Span}";
        }
    }
}
