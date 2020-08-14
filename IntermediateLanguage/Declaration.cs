using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    // TODO consider naming these just Class, Function, Field, Constructor. While they are declarations, the IL is what we think of as actually being the thing of which the code is declaring
    [Closed(
        typeof(ClassDeclaration),
        typeof(FunctionDeclaration),
        typeof(MethodDeclaration),
        typeof(FieldDeclaration),
        typeof(ConstructorDeclaration))]
    public abstract class Declaration
    {
        public bool IsMember { get; }
        public MaybeQualifiedName FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;
        public Symbol Symbol { get; }

        protected Declaration(bool isMember, MaybeQualifiedName fullName, Symbol symbol)
        {
            IsMember = isMember;
            FullName = fullName;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return FullName.ToString();
        }
    }
}
