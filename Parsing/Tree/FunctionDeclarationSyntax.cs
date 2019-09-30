using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [Closed(
        typeof(NamedFunctionDeclarationSyntax),
        typeof(ConstructorDeclarationSyntax))]
    internal abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax, IFunctionDeclarationSyntax
    {
        private DataType? selfParameterType;
        [DisallowNull]
        public DataType? SelfParameterType
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
        public virtual FixedList<StatementSyntax>? Body { get; }
        public TypePromise ReturnType { get; } = new TypePromise();

        [DisallowNull]
        public ControlFlowGraph? ControlFlow { get; set; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();

        protected FunctionDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<ParameterSyntax> parameters,
            FixedList<StatementSyntax>? body)
            : base(span, file, fullName, nameSpan,
                new SymbolSet(GetChildSymbols(parameters, body)))
        {
            Modifiers = modifiers;
            Parameters = parameters;
            Body = body;
        }

        private static IEnumerable<ISymbol> GetChildSymbols(
             FixedList<ParameterSyntax> parameters,
             FixedList<StatementSyntax>? body)
        {
            var variableDeclarations = GetVariableDeclarations(body);
            return ((IEnumerable<ISymbol>)parameters).Concat(variableDeclarations);
        }

        private static IReadOnlyList<VariableDeclarationStatementSyntax> GetVariableDeclarations(
            FixedList<StatementSyntax>? body)
        {
            var visitor = new GetVariableDeclarationsVisitor();
            if (body != null)
                foreach (var statement in body)
                    visitor.VisitStatement(statement, default);
            var variableDeclarations = visitor.VariableDeclarations;
            return variableDeclarations;
        }

        public IReadOnlyList<VariableDeclarationStatementSyntax> GetVariableDeclarations()
        {
            return GetVariableDeclarations(Body);
        }
    }
}
