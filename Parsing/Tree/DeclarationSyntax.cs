using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class DeclarationSyntax : Syntax, IDeclarationSyntax
    {
        private LexicalScope? containingLexicalScope;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get => containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }

        public CodeFile File { get; }
        public Name? Name { get; }
        public TextSpan NameSpan { get; }
        public IPromise<Symbol> Symbol { get; }
        protected DeclarationSyntax(TextSpan span, CodeFile file, Name? name, TextSpan nameSpan, IPromise<Symbol> symbol)
            : base(span)
        {
            NameSpan = nameSpan;
            Symbol = symbol;
            File = file;
            Name = name;
        }
    }
}
