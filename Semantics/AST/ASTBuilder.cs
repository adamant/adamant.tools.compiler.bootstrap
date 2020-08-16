using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST
{
    // ReSharper disable once UnusedMember.Global
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes",
        Justification = "In Progress")]
    internal class ASTBuilder
    {
        // ReSharper disable once UnusedMember.Global
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public Package BuildPackage(FixedList<IEntityDeclarationSyntax> entities)
        {
            var nonMemberDeclarations = entities
                                        .OfType<INonMemberEntityDeclarationSyntax>()
                                        .Select(BuildNonMemberDeclaration).ToFixedList();

            return new Package(nonMemberDeclarations);
        }

        private static INonMemberDeclaration BuildNonMemberDeclaration(INonMemberEntityDeclarationSyntax entity)
        {
            return entity switch
            {
                IClassDeclarationSyntax syn => BuildClass(syn),
                IFunctionDeclarationSyntax syn => BuildFunction(syn),
                _ => throw ExhaustiveMatch.Failed(entity)
            };
        }

        private static IClassDeclaration BuildClass(IClassDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var members = syn.Members.Select(BuildMember).ToFixedList();
            return new ClassDeclaration(syn.Span, symbol, members);
        }

        private static IMemberDeclaration BuildMember(IMemberDeclarationSyntax member)
        {
            return member switch
            {
                IAssociatedFunctionDeclarationSyntax syn => BuildAssociatedFunction(syn),
                IAbstractMethodDeclarationSyntax syn => BuildAbstractMethod(syn),
                IConcreteMethodDeclarationSyntax syn => BuildConcreteMethod(syn),
                IConstructorDeclarationSyntax syn => BuildConstructor(syn),
                IFieldDeclarationSyntax syn => BuildField(syn),
                _ => throw ExhaustiveMatch.Failed(member)
            };
        }

        private static IAssociatedFunctionDeclaration BuildAssociatedFunction(IAssociatedFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new AssociatedFunctionDeclaration(syn.Span, symbol, parameters, body);
        }

        private static IAbstractMethodDeclaration BuildAbstractMethod(IAbstractMethodDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.SelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            return new AbstractMethodDeclaration(syn.Span, symbol, selfParameter, parameters);
        }

        private static IConcreteMethodDeclaration BuildConcreteMethod(IConcreteMethodDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.SelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new ConcreteMethodDeclaration(syn.Span, symbol, selfParameter, parameters, body);
        }

        private static IConstructorDeclaration BuildConstructor(IConstructorDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.ImplicitSelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new ConstructorDeclaration(syn.Span, symbol, selfParameter, parameters, body);
        }

        private static IFieldDeclaration BuildField(IFieldDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            return new FieldDeclaration(syn.Span, symbol);
        }

        private static IFunctionDeclaration BuildFunction(IFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new FunctionDeclaration(syn.Span, symbol, parameters, body);
        }

        private static IConstructorParameter BuildParameter(IConstructorParameterSyntax parameter)
        {
            return parameter switch
            {
                INamedParameterSyntax syn => BuildParameter(syn),
                IFieldParameterSyntax syn => BuildParameter(syn),
                _ => throw ExhaustiveMatch.Failed(parameter),
            };
        }

        private static INamedParameter BuildParameter(INamedParameterSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var defaultValue = BuildExpression(syn.DefaultValue);
            return new NamedParameter(syn.Span, symbol, syn.Unused, defaultValue);
        }

        private static ISelfParameter BuildParameter(ISelfParameterSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var unused = syn.Unused;
            return new SelfParameter(syn.Span, symbol, unused);
        }

        private static IFieldParameter BuildParameter(IFieldParameterSyntax syn)
        {
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var defaultValue = BuildExpression(syn.DefaultValue);
            return new FieldParameter(syn.Span, referencedSymbol, defaultValue);
        }

        private static IBody BuildBody(IBodySyntax syn)
        {
            var statements = syn.Statements.Select(BuildBodyStatement).ToFixedList();
            return new Body(syn.Span, statements);
        }

        private static IStatement BuildStatement(IStatementSyntax stmt)
        {
            return stmt switch
            {
                IResultStatementSyntax syn => BuildResultStatement(syn),
                IBodyStatementSyntax syn => BuildBodyStatement(syn),
                _ => throw ExhaustiveMatch.Failed(stmt),
            };
        }

        private static IBodyStatement BuildBodyStatement(IBodyStatementSyntax stmt)
        {
            return stmt switch
            {
                IExpressionStatementSyntax syn => BuildExpressionStatement(syn),
                IVariableDeclarationStatementSyntax syn => BuildVariableDeclaration(syn),
                _ => throw ExhaustiveMatch.Failed(stmt),
            };
        }

        private static IExpressionStatement BuildExpressionStatement(IExpressionStatementSyntax syn)
        {
            var expression = BuildExpression(syn.Expression);
            return new ExpressionStatement(syn.Span, expression);
        }

        private static IVariableDeclarationStatement BuildVariableDeclaration(IVariableDeclarationStatementSyntax syn)
        {
            var nameSpan = syn.NameSpan;
            var symbol = syn.Symbol.Result;
            var initializer = BuildExpression(syn.Initializer);
            return new VariableDeclarationStatement(syn.Span, nameSpan, symbol, initializer);
        }

        private static IResultStatement BuildResultStatement(IResultStatementSyntax syn)
        {
            var expression = BuildExpression(syn.Expression);
            return new ResultStatement(syn.Span, expression);
        }

        private static IBlockOrResult BuildBlockOrResult(IBlockOrResultSyntax syntax)
        {
            return syntax switch
            {
                IBlockExpressionSyntax syn => BuildBlockExpression(syn),
                IResultStatementSyntax syn => BuildResultStatement(syn),
                _ => throw ExhaustiveMatch.Failed(syntax),
            };
        }



        [return: NotNullIfNotNull("expression")]
        private static IExpression? BuildExpression(IExpressionSyntax? expression)
        {
            return expression switch
            {
                null => null,
                IAssignmentExpressionSyntax syn => BuildAssignmentExpression(syn),
                IBinaryOperatorExpressionSyntax syn => BuildBinaryOperatorExpression(syn),
                IBlockExpressionSyntax syn => BuildBlockExpression(syn),
                IBoolLiteralExpressionSyntax syn => BuildBoolLiteralExpression(syn),
                IBorrowExpressionSyntax syn => BuildBorrowExpression(syn),
                IBreakExpressionSyntax syn => BuildBreakExpression(syn),
                IFieldAccessExpressionSyntax syn => BuildFieldAccessExpression(syn),
                IForeachExpressionSyntax syn => BuildForeachExpression(syn),
                IFunctionInvocationExpressionSyntax syn => BuildFunctionInvocationExpression(syn),
                IIfExpressionSyntax syn => BuildIfExpression(syn),
                IImplicitImmutabilityConversionExpressionSyntax syn => BuildImplicitImmutabilityConversionExpression(syn),
                IImplicitNoneConversionExpressionSyntax syn => BuildImplicitNoneConversionExpression(syn),
                IImplicitNumericConversionExpressionSyntax syn => BuildImplicitNumericConversionExpression(syn),
                IImplicitOptionalConversionExpressionSyntax syn => BuildImplicitOptionalConversionExpression(syn),
                IIntegerLiteralExpressionSyntax syn => BuildIntegerLiteralExpression(syn),
                INoneLiteralExpressionSyntax syn => BuildNoneLiteralExpression(syn),
                IStringLiteralExpressionSyntax syn => BuildStringLiteralExpression(syn),
                ILoopExpressionSyntax syn => BuildLoopExpression(syn),
                IMethodInvocationExpressionSyntax syn => BuildMethodInvocationExpression(syn),
                IMoveExpressionSyntax syn => BuildMoveExpression(syn),
                INameExpressionSyntax syn => BuildNameExpression(syn),
                INewObjectExpressionSyntax syn => BuildNewObjectExpression(syn),
                INextExpressionSyntax syn => BuildNextExpression(syn),
                IReturnExpressionSyntax syn => BuildReturnExpression(syn),
                ISelfExpressionSyntax syn => BuildSelfExpression(syn),
                IShareExpressionSyntax syn => BuildShareExpression(syn),
                IUnaryOperatorExpressionSyntax syn => BuildUnaryOperatorExpression(syn),
                IUnsafeExpressionSyntax syn => BuildUnsafeExpression(syn),
                IWhileExpressionSyntax syn => BuildWhileExpression(syn),
                _ => throw ExhaustiveMatch.Failed(expression),
            };
        }

        private static IAssignmentExpression BuildAssignmentExpression(IAssignmentExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var leftOperand = BuildAssignableExpression(syn.LeftOperand);
            var @operator = syn.Operator;
            var rightOperand = BuildExpression(syn.RightOperand);
            return new AssignmentExpression(syn.Span, type, leftOperand, @operator, rightOperand);
        }

        private static IAssignableExpression BuildAssignableExpression(IAssignableExpressionSyntax expression)
        {
            return expression switch
            {
                IFieldAccessExpressionSyntax syn => BuildFieldAccessExpression(syn),
                INameExpressionSyntax syn => BuildNameExpression(syn),
                _ => throw ExhaustiveMatch.Failed(expression),
            };
        }

        private static IBinaryOperatorExpression BuildBinaryOperatorExpression(IBinaryOperatorExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var leftOperand = BuildExpression(syn.LeftOperand);
            var @operator = syn.Operator;
            var rightOperand = BuildExpression(syn.RightOperand);
            return new BinaryOperatorExpression(syn.Span, type, leftOperand, @operator, rightOperand);
        }

        private static IBlockExpression BuildBlockExpression(IBlockExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var statements = syn.Statements.Select(BuildStatement).ToFixedList();
            return new BlockExpression(syn.Span, type, statements);
        }

        private static IBoolLiteralExpression BuildBoolLiteralExpression(IBoolLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var value = syn.Value;
            return new BoolLiteralExpression(syn.Span, type, value);
        }

        private static IBorrowExpression BuildBorrowExpression(IBorrowExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var referent = BuildExpression(syn.Referent);
            return new BorrowExpression(syn.Span, type, referencedSymbol, referent);
        }

        private static IBreakExpression BuildBreakExpression(IBreakExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var value = BuildExpression(syn.Value);
            return new BreakExpression(syn.Span, type, value);
        }

        private static IFieldAccessExpression BuildFieldAccessExpression(IFieldAccessExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var context = BuildExpression(syn.Context);
            var accessOperator = syn.AccessOperator;
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            return new FieldAccessExpression(syn.Span, type, context, accessOperator, referencedSymbol);
        }

        private static IForeachExpression BuildForeachExpression(IForeachExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var symbol = syn.Symbol.Result;
            var inExpression = BuildExpression(syn.InExpression);
            var block = BuildBlockExpression(syn.Block);
            return new ForeachExpression(syn.Span, type, symbol, inExpression, block);
        }

        private static IFunctionInvocationExpression BuildFunctionInvocationExpression(IFunctionInvocationExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            FixedList<IExpression> arguments = syn.Arguments.Select(a => BuildExpression(a.Expression)).ToFixedList();
            return new FunctionInvocationExpression(syn.Span, type, referencedSymbol, arguments);
        }

        private static IIfExpression BuildIfExpression(IIfExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var condition = BuildExpression(syn.Condition);
            var thenBlock = BuildBlockOrResult(syn.ThenBlock);
            var elseClause = BuildElseClause(syn.ElseClause);
            return new IfExpression(syn.Span, type, condition, thenBlock, elseClause);
        }

        [return: NotNullIfNotNull("syn")]
        private static IElseClause? BuildElseClause(IElseClauseSyntax? elseClause)
        {
            return elseClause switch
            {
                null => null,
                IBlockOrResultSyntax syn => BuildBlockOrResult(syn),
                IIfExpressionSyntax syn => BuildIfExpression(syn),
                _ => throw ExhaustiveMatch.Failed(elseClause),
            };
        }

        private static IImplicitImmutabilityConversionExpression BuildImplicitImmutabilityConversionExpression(
            IImplicitImmutabilityConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitImmutabilityConversion(syn.Span, type, expression, convertToType);

        }

        private static IImplicitNoneConversionExpression BuildImplicitNoneConversionExpression(IImplicitNoneConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitNoneConversionExpression(syn.Span, type, expression, convertToType);
        }

        private static IImplicitNumericConversionExpression BuildImplicitNumericConversionExpression(IImplicitNumericConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitNumericConversionExpression(syn.Span, type, expression, convertToType);
        }

        private static IImplicitOptionalConversionExpression BuildImplicitOptionalConversionExpression(IImplicitOptionalConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitOptionalConversionExpression(syn.Span, type, expression, convertToType);
        }

        private static IIntegerLiteralExpression BuildIntegerLiteralExpression(IIntegerLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var value = syn.Value;
            return new IntegerLiteralExpression(syn.Span, type, value);
        }

        private static INoneLiteralExpression BuildNoneLiteralExpression(INoneLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            return new NoneLiteralExpression(syn.Span, type);
        }

        private static IStringLiteralExpression BuildStringLiteralExpression(IStringLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var value = syn.Value;
            return new StringLiteralExpression(syn.Span, type, value);
        }

        private static ILoopExpression BuildLoopExpression(ILoopExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var block = BuildBlockExpression(syn.Block);
            return new LoopExpression(syn.Span, type, block);
        }

        private static IMethodInvocationExpression BuildMethodInvocationExpression(IMethodInvocationExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var context = BuildExpression(syn.Context);
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            FixedList<IExpression> arguments = syn.Arguments.Select(a => BuildExpression(a.Expression)).ToFixedList();
            return new MethodInvocationExpression(syn.Span, type, context, referencedSymbol, arguments);
        }

        private static IMoveExpression BuildMoveExpression(IMoveExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var referent = BuildExpression(syn.Referent);
            return new MoveExpression(syn.Span, type, referencedSymbol, referent);
        }

        private static INameExpression BuildNameExpression(INameExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            return new NameExpression(syn.Span, type, referencedSymbol);
        }

        private static INewObjectExpression BuildNewObjectExpression(INewObjectExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            FixedList<IExpression> arguments = syn.Arguments.Select(a => BuildExpression(a.Expression)).ToFixedList();
            return new NewObjectExpression(syn.Span, type, referencedSymbol, arguments);
        }

        private static INextExpression BuildNextExpression(INextExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            return new NextExpression(syn.Span, type);
        }

        private static IReturnExpression BuildReturnExpression(IReturnExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var value = BuildExpression(syn.Value);
            return new ReturnExpression(syn.Span, type, value);
        }

        private static ISelfExpression BuildSelfExpression(ISelfExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var isImplicit = syn.IsImplicit;
            return new SelfExpression(syn.Span, type, referencedSymbol, isImplicit);
        }

        private static IShareExpression BuildShareExpression(IShareExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var referent = BuildExpression(syn.Referent);
            return new ShareExpression(syn.Span, type, referencedSymbol, referent);
        }

        private static IUnaryOperatorExpression BuildUnaryOperatorExpression(IUnaryOperatorExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var fixity = syn.Fixity;
            var @operator = syn.Operator;
            var operand = BuildExpression(syn.Operand);
            return new UnaryOperatorExpression(syn.Span, type, fixity, @operator, operand);
        }

        private static IUnsafeExpression BuildUnsafeExpression(IUnsafeExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var expression = BuildExpression(syn.Expression);
            return new UnsafeExpression(syn.Span, type, expression);
        }

        private static IWhileExpression BuildWhileExpression(IWhileExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var condition = BuildExpression(syn.Condition);
            var block = BuildBlockExpression(syn.Block);
            return new WhileExpression(syn.Span, type, condition, block);
        }
    }
}
