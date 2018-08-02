namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class SemanticAttribute
    {
        public abstract string AttributeKey { get; }

        protected readonly SemanticAttributes Attributes;
        protected TypeAttribute Type => Attributes.Type;
        protected ParentAttribute Parent => Attributes.Parent;
        protected NameAttribute Name => Attributes.Name;
        protected SyntaxSymbolAttribute SyntaxSymbol => Attributes.SyntaxSymbol;
        protected LexicalScopeAttribute NameScope => Attributes.NameScope;

        protected SemanticAttribute(SemanticAttributes attributes)
        {
            Attributes = attributes;
        }
    }
}
