using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    /// <summary>
    /// Used within the parse to represent a name that we aren't yet sure whether
    /// it is a name expression, or a callable name
    /// </summary>
    internal struct NameSyntax
    {
        public TextSpan Span { get; }
        public SimpleName Name { get; }

        public NameSyntax(TextSpan span, SimpleName name)
        {
            Span = span;
            Name = name;
        }

        public NameExpressionSyntax ToExpression()
        {
            return new NameExpressionSyntax(Span, Name);
        }

        public CallableNameSyntax ToCallable()
        {
            return new CallableNameSyntax(Span, Name);
        }
    }
}
