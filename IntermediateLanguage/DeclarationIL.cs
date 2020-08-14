using Adamant.Tools.Compiler.Bootstrap.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    // TODO consider naming these just Class, Function, Field, Constructor. While they are declarations, the IL is what we think of as actually being the thing of which the code is declaring
    [Closed(
        typeof(ClassIL),
        typeof(FunctionIL),
        typeof(MethodDeclarationIL),
        typeof(FieldIL),
        typeof(ConstructorIL))]
    public abstract class DeclarationIL
    {
        public bool IsMember { get; }
        public Symbol Symbol { get; }

        protected DeclarationIL(bool isMember, Symbol symbol)
        {
            IsMember = isMember;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}
