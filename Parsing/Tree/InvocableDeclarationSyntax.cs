using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InvocableDeclarationSyntax : DeclarationSyntax, IInvocableDeclarationSyntax
    {
        public IAccessModifierToken? AccessModifier { get; }
        public MaybeQualifiedName FullName { get; }
        public FixedList<IConstructorParameterSyntax> Parameters { get; }
        public FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }
        public new IPromise<InvocableSymbol> Symbol { get; }

        protected InvocableDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name? name,
            IEnumerable<IConstructorParameterSyntax> parameters,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IPromise<InvocableSymbol> symbol)
            : base(span, file, name, nameSpan, symbol)
        {
            AccessModifier = accessModifier;
            FullName = fullName;
            Parameters = parameters.ToFixedList();
            ReachabilityAnnotations = reachabilityAnnotations;
            Symbol = symbol;
        }
    }
}
