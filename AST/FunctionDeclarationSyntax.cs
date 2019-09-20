using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(NamedFunctionDeclarationSyntax),
        typeof(ConstructorDeclarationSyntax))]
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax, IFunctionSymbol
    {
        private DataType selfParameterType;
        public DataType SelfParameterType
        {
            get => selfParameterType;
            set
            {
                if (selfParameterType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                selfParameterType = value ?? throw new ArgumentException();
            }
        }

        public FixedList<IModiferToken> Modifiers { get; }
        public FixedList<ParameterSyntax> Parameters { get; }
        public BlockSyntax Body { get; }
        public TypePromise ReturnType { get; } = new TypePromise();
        public ControlFlowGraph ControlFlow { get; set; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();

        protected FunctionDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<ParameterSyntax> parameters,
            BlockSyntax body)
            : base(file, fullName, nameSpan,
                new SymbolSet(GetChildSymbols(parameters, body)))
        {
            Modifiers = modifiers;
            Parameters = parameters;
            Body = body;
        }

        private static IEnumerable<ISymbol> GetChildSymbols(
             FixedList<ParameterSyntax> parameters,
             BlockSyntax body)
        {
            var variableDeclarations = GetVariableDeclarations(body);
            return ((IEnumerable<ISymbol>)parameters).Concat(variableDeclarations);
        }

        private static IReadOnlyList<VariableDeclarationStatementSyntax> GetVariableDeclarations(BlockSyntax body)
        {
            var visitor = new GetVariableDeclarationsVisitor();
            visitor.VisitExpression(body, default);
            var variableDeclarations = visitor.VariableDeclarations;
            return variableDeclarations;
        }

        public IReadOnlyList<VariableDeclarationStatementSyntax> GetVariableDeclarations()
        {
            return GetVariableDeclarations(Body);
        }
    }
}
