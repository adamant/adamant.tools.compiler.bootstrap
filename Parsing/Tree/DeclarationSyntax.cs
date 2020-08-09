using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class DeclarationSyntax : Syntax, IDeclarationSyntax
    {
        private LexicalScope<IPromise<Symbol>>? containingLexicalScope;
        public LexicalScope<IPromise<Symbol>> ContainingLexicalScope
        {
            get =>
                containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }

        public CodeFile File { get; }
        public TextSpan NameSpan { get; }

        protected DeclarationSyntax(TextSpan span, CodeFile file, TextSpan nameSpan)
            : base(span)
        {
            NameSpan = nameSpan;
            File = file;
        }
    }
}
