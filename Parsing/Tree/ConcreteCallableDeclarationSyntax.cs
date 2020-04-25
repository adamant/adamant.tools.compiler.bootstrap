using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class ConcreteCallableDeclarationSyntax : CallableDeclarationSyntax
    {
        public virtual IBodySyntax Body { get; }

        protected ConcreteCallableDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<IParameterSyntax> parameters,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, modifiers, fullName, nameSpan, parameters, reachabilityAnnotations,
                GetChildSymbols(parameters, body))
        {
            Body = body;
        }

        internal static SymbolSet GetChildSymbols(
            IEnumerable<IParameterSyntax> parameters,
            IBodySyntax body)
        {
            var variableDeclarations = body.GetAllVariableDeclarations()
                                       ?? Enumerable.Empty<IVariableDeclarationStatementSyntax>();
            var childSymbols = parameters.Cast<ISymbol>().Concat(variableDeclarations);
            return new SymbolSet(childSymbols);
        }
    }
}
