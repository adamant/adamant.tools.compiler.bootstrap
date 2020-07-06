using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class CallableDeclarationSyntax : DeclarationSyntax, ICallableDeclarationSyntax
    {
        public Name FullName { get; }
        private DataType? selfParameterType;
        [DisallowNull]
        public DataType? SelfParameterType
        {
            get => selfParameterType;
            set
            {
                if (selfParameterType != null)
                    throw new InvalidOperationException("Can't set SelfParameterType repeatedly");
                selfParameterType = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public FixedList<IModiferToken> Modifiers { get; }
        public FixedList<IParameterSyntax> Parameters { get; }
        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;
        public TypePromise ReturnType { get; } = new TypePromise();
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();
        public FixedList<IReachabilityAnnotationSyntax> ReachabilityAnnotations { get; }

        public SymbolSet ChildSymbols { get; protected set; }

        protected CallableDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            IEnumerable<IParameterSyntax> parameters,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            SymbolSet childSymbols)
            : base(span, file, nameSpan)
        {
            FullName = fullName;
            Modifiers = modifiers;
            Parameters = parameters.ToFixedList();
            ReachabilityAnnotations = reachabilityAnnotations;
            ChildSymbols = childSymbols;
        }

        protected static SymbolSet GetChildSymbols(
            ISelfParameterSyntax? selfParameter,
            IEnumerable<IParameterSyntax> parameters,
            IBodySyntax? body)
        {
            if (!(selfParameter is null))
                parameters = parameters.Prepend(selfParameter);

            var variableDeclarations = body?.GetAllVariableDeclarations()
                                       ?? Enumerable.Empty<IBindingSymbol>();
            var childSymbols = parameters.Concat(variableDeclarations);
            return new SymbolSet(childSymbols);
        }
    }
}
