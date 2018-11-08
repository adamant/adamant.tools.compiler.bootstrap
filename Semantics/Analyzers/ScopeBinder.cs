using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class ScopeBinder
    {
        [NotNull] private readonly Dictionary<DeclarationSyntax, DeclarationAnalysis> declarations = new Dictionary<DeclarationSyntax, DeclarationAnalysis>();
        [NotNull] private readonly Dictionary<string, IDeclarationAnalysis> globalDeclarations;
        [NotNull] private readonly NameBuilder nameBuilder;
        [NotNull] [ItemNotNull] private readonly IReadOnlyList<Declaration> referencedDeclarations;

        public ScopeBinder(
            [NotNull] [ItemNotNull] IEnumerable<CompilationUnitAnalysis> compilationUnits,
            [NotNull] NameBuilder nameBuilder,
            [NotNull] IReadOnlyDictionary<string, Package> references)
        {
            this.nameBuilder = nameBuilder;
            referencedDeclarations = references.SelectMany(r => r.Value.Declarations).ToReadOnlyList();

            foreach (var compilationUnit in compilationUnits)
                GatherDeclarations(compilationUnit.Namespace);

            globalDeclarations = declarations.Values.AssertNotNull()
                .OfType<IDeclarationAnalysis>()
                .Where(IsGlobalDeclaration)
                .ToDictionary(d => d.Name.UnqualifiedName.Text, d => d);
        }

        private void GatherDeclarations([NotNull] DeclarationAnalysis declaration)
        {
            declarations.Add(declaration.Syntax, declaration);
            if (!(declaration is NamespaceDeclarationAnalysis namespaceDeclaration)) return;
            foreach (var nestDeclaration in namespaceDeclaration.Declarations)
                GatherDeclarations(nestDeclaration);
        }

        private static bool IsGlobalDeclaration([NotNull] ISymbol declaration)
        {
            return declaration.Name is SimpleName;
        }

        public void BindCompilationUnitScope([NotNull] CompilationUnitScope scope)
        {
            Requires.NotNull(nameof(scope), scope);
            scope.Bind(globalDeclarations.ToDictionary(i => i.Key, i => i.Value as ISymbol));
            foreach (var nestedScope in scope.NestedScopes)
                BindScope(nestedScope);
        }

        private void BindScope([NotNull] LexicalScope scope)
        {
            Requires.NotNull(nameof(scope), scope);
            var symbols = new Dictionary<string, ISymbol>();
            switch (scope)
            {
                case FunctionScope functionScope:
                {
                    var function =
                        (FunctionDeclarationAnalysis)declarations[functionScope.Syntax]
                            .AssertNotNull();

                    foreach (var parameter in function.Parameters)
                        AddSymbol(symbols, parameter);

                    foreach (var declaration in function.Statements
                        .OfType<VariableDeclarationStatementAnalysis>())
                        AddSymbol(symbols, declaration);

                    functionScope.Bind(symbols);

                    var blocks = new Dictionary<ExpressionSyntax, ILocalVariableScopeAnalysis>();
                    GetVariableScopes(function.Statements, blocks);
                    var bodyScope = functionScope.NestedScopes.Cast<LocalVariableScope>()
                        .SingleOrDefault();
                    if (bodyScope != null)
                        BindBlockScope(bodyScope, blocks);
                }
                break;
                case NamespaceScope namespaceScope:
                {
                    AddSymbolsInNamespace(symbols, namespaceScope.Name);
                    namespaceScope.Bind(symbols);
                    foreach (var nestedScope in namespaceScope.NestedScopes)
                        BindScope(nestedScope);
                }
                break;
                case GenericsScope genericsScope:
                {
                    var declaration = (MemberDeclarationAnalysis)declarations[genericsScope.Syntax].AssertNotNull();
                    foreach (var parameter in declaration.GenericParameters.AssertNotNull())
                        AddSymbol(symbols, parameter);

                    genericsScope.Bind(symbols);

                    foreach (var nestedScope in genericsScope.NestedScopes)
                        BindScope(nestedScope);
                }
                break;
                case UsingDirectivesScope usingDirectivesScope:
                {
                    var declaration = (NamespaceDeclarationAnalysis)declarations[usingDirectivesScope.Syntax].AssertNotNull();
                    foreach (var usingDirective in declaration.Syntax.UsingDirectives)
                    {
                        var usingNamespace = nameBuilder.BuildName(usingDirective.Name).AssertNotNull();
                        AddSymbolsInNamespace(symbols, usingNamespace);
                    }

                    usingDirectivesScope.Bind(symbols);

                    foreach (var nestedScope in usingDirectivesScope.NestedScopes)
                        BindScope(nestedScope);
                }
                break;
                default:
                    throw NonExhaustiveMatchException.For(scope);
            }
        }

        private void AddSymbolsInNamespace([NotNull] Dictionary<string, ISymbol> symbols, [NotNull]  Name namespaceName)
        {
            foreach (var importedDeclaration in declarations.Values
                .OfType<IDeclarationAnalysis>()
                .Where(d => d.Name.HasQualifier(namespaceName)))
            {
                AddSymbol(symbols, importedDeclaration);
            }

            foreach (var importedDeclaration in referencedDeclarations.Where(d =>
                d.Name.HasQualifier(namespaceName)))
            {
                AddSymbol(symbols, importedDeclaration);
            }
        }

        private static void AddSymbol([NotNull] Dictionary<string, ISymbol> symbols, [NotNull] ISymbol symbol)
        {
            var name = symbol.Name.UnqualifiedName.Text;
            if (symbols.TryGetValue(name, out var existingSymbol))
                symbols[name] = existingSymbol.ComposeWith(symbol);
            else
                symbols.Add(name, symbol);
        }

        private static void GetVariableScopes(
            [NotNull] IReadOnlyList<StatementAnalysis> statements,
            [NotNull] Dictionary<ExpressionSyntax, ILocalVariableScopeAnalysis> scopes)
        {
            foreach (var statement in statements)
                switch (statement)
                {
                    case ExpressionStatementAnalysis expressionStatement:
                        GetVariableScopes(expressionStatement.Expression, scopes);
                        break;
                    case VariableDeclarationStatementAnalysis variableDeclaration:
                        if (variableDeclaration.Initializer != null)
                            GetVariableScopes(variableDeclaration.Initializer, scopes);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(statement);
                }
        }

        private static void GetVariableScopes(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] Dictionary<ExpressionSyntax, ILocalVariableScopeAnalysis> scopes)
        {
            switch (expression)
            {
                case BlockAnalysis block:
                    scopes.Add(block.Syntax, block);
                    GetVariableScopes(block.Statements, scopes);
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        GetVariableScopes(returnExpression.ReturnExpression, scopes);
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    GetVariableScopes(binaryOperatorExpression.LeftOperand, scopes);
                    GetVariableScopes(binaryOperatorExpression.RightOperand, scopes);
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    GetVariableScopes(unaryOperatorExpression.Operand, scopes);
                    break;
                case ForeachExpressionAnalysis foreachExpression:
                    // The foreach expression itself declares a variable
                    scopes.Add(foreachExpression.Syntax, foreachExpression);
                    GetVariableScopes(foreachExpression.InExpression, scopes);
                    GetVariableScopes(foreachExpression.Block, scopes);
                    break;
                case WhileExpressionAnalysis whileExpression:
                    GetVariableScopes(whileExpression.Condition, scopes);
                    GetVariableScopes(whileExpression.Block, scopes);
                    break;
                case LoopExpressionAnalysis loopExpression:
                    GetVariableScopes(loopExpression.Block, scopes);
                    break;
                case InvocationAnalysis invocation:
                    foreach (var argument in invocation.Arguments)
                        GetVariableScopes(argument.Value, scopes);
                    break;
                case GenericInvocationAnalysis genericInvocation:
                    foreach (var argument in genericInvocation.Arguments)
                        GetVariableScopes(argument.Value, scopes);
                    break;
                case NewObjectExpressionAnalysis newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        GetVariableScopes(argument.Value, scopes);
                    break;
                case PlacementInitExpressionAnalysis initStructExpression:
                    foreach (var argument in initStructExpression.Arguments)
                        GetVariableScopes(argument.Value, scopes);
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    GetVariableScopes(unsafeExpression.Expression, scopes);
                    break;
                case RefTypeAnalysis refType:
                    GetVariableScopes(refType.ReferencedType, scopes);
                    break;
                case IfExpressionAnalysis ifExpression:
                    GetVariableScopes(ifExpression.Condition, scopes);
                    GetVariableScopes(ifExpression.ThenBlock, scopes);
                    if (ifExpression.ElseClause != null)
                        GetVariableScopes(ifExpression.ElseClause, scopes);
                    break;
                case ResultExpressionAnalysis resultExpression:
                    GetVariableScopes(resultExpression.Expression, scopes);
                    break;
                case BreakExpressionAnalysis breakExpression:
                    if (breakExpression.Expression != null)
                        GetVariableScopes(breakExpression.Expression, scopes);
                    break;
                case IntegerLiteralExpressionAnalysis _:
                case IdentifierNameAnalysis _:
                case BooleanLiteralExpressionAnalysis _:
                case StringLiteralExpressionAnalysis _:
                case PrimitiveTypeAnalysis _:
                case UninitializedExpressionAnalysis _:
                    // Do nothing
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void BindBlockScope(
            [NotNull] LocalVariableScope scope,
            [NotNull] Dictionary<ExpressionSyntax, ILocalVariableScopeAnalysis> scopes)
        {
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(scopes), scopes);

            var scopeAnalysis = scopes[scope.Syntax].AssertNotNull();
            var variableDeclarations = new Dictionary<string, ISymbol>();
            foreach (var declaration in scopeAnalysis.LocalVariableDeclarations())
                AddSymbol(variableDeclarations, declaration);

            scope.Bind(variableDeclarations);

            foreach (var nestedScope in scope.NestedScopes.Cast<LocalVariableScope>())
                BindBlockScope(nestedScope, scopes);
        }
    }
}
