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
        public virtual FixedList<IStatementSyntax>? Body { get; }
        public TypePromise ReturnType { get; } = new TypePromise();
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();

        public SymbolSet ChildSymbols { get; protected set; }

        protected CallableDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<IParameterSyntax> parameters,
            FixedList<IStatementSyntax>? body)
            : base(span, file, nameSpan)
        {
            FullName = fullName;
            Modifiers = modifiers;
            Parameters = parameters;
            Body = body;
            ChildSymbols = new SymbolSet(GetChildSymbols(parameters, body));
        }

        private static IEnumerable<ISymbol> GetChildSymbols(
             FixedList<IParameterSyntax> parameters,
             FixedList<IStatementSyntax>? body)
        {
            var variableDeclarations = body?.GetAllVariableDeclarations() ?? Enumerable.Empty<IVariableDeclarationStatementSyntax>();
            return ((IEnumerable<ISymbol>)parameters).Concat(variableDeclarations);
        }
    }
}
