using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IHasContainingLexicalScope
    {
        LexicalScope ContainingLexicalScope { get; set; }
    }
}
