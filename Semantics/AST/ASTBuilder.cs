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

            FixedList<IMemberDeclaration> BuildMembers(IClassDeclaration c)
                => syn.Members.Select(m => BuildMember(c, m)).ToFixedList();

            return new ClassDeclaration(syn.File, syn.Span, symbol, BuildMembers);
        }

        private static IMemberDeclaration BuildMember(
            IClassDeclaration declaringClass,
            IMemberDeclarationSyntax member)
        {
            return member switch
            {
                IAssociatedFunctionDeclarationSyntax syn => BuildAssociatedFunction(declaringClass, syn),
                IAbstractMethodDeclarationSyntax syn => BuildAbstractMethod(declaringClass, syn),
                IConcreteMethodDeclarationSyntax syn => BuildConcreteMethod(declaringClass, syn),
                IConstructorDeclarationSyntax syn => BuildConstructor(declaringClass, syn),
                IFieldDeclarationSyntax syn => BuildField(declaringClass, syn),
                _ => throw ExhaustiveMatch.Failed(member)
            };
        }

        private static IAssociatedFunctionDeclaration BuildAssociatedFunction(
            IClassDeclaration declaringClass,
            IAssociatedFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new AssociatedFunctionDeclaration(syn.File, syn.Span, declaringClass, symbol, parameters, body);
        }

        private static IAbstractMethodDeclaration BuildAbstractMethod(
            IClassDeclaration declaringClass,
            IAbstractMethodDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.SelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            return new AbstractMethodDeclaration(syn.File, syn.Span, declaringClass, symbol, selfParameter, parameters);
        }

        private static IConcreteMethodDeclaration BuildConcreteMethod(
            IClassDeclaration declaringClass,
            IConcreteMethodDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.SelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new ConcreteMethodDeclaration(syn.File, syn.Span, declaringClass, symbol, selfParameter, parameters, body);
        }

        private static IConstructorDeclaration BuildConstructor(
            IClassDeclaration declaringClass,
            IConstructorDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.ImplicitSelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new ConstructorDeclaration(syn.File, syn.Span, declaringClass, symbol, selfParameter, parameters, body);
        }

        private static IFieldDeclaration BuildField(
            IClassDeclaration declaringClass,
            IFieldDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            return new FieldDeclaration(syn.File, syn.Span, declaringClass, symbol);
        }

        private static IFunctionDeclaration BuildFunction(IFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            IBody body = BuildBody(syn.Body);
            return new FunctionDeclaration(syn.File, syn.Span, symbol, parameters, body);
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
            var semantics = syn.Semantics.Assigned();
            var leftOperand = BuildAssignableExpression(syn.LeftOperand);
            var @operator = syn.Operator;
            var rightOperand = BuildExpression(syn.RightOperand);
            return new AssignmentExpression(syn.Span, type, semantics, leftOperand, @operator, rightOperand);
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
            var semantics = syn.Semantics.Assigned();
            var leftOperand = BuildExpression(syn.LeftOperand);
            var @operator = syn.Operator;
            var rightOperand = BuildExpression(syn.RightOperand);
            return new BinaryOperatorExpression(syn.Span, type, semantics, leftOperand, @operator, rightOperand);
        }

        private static IBlockExpression BuildBlockExpression(IBlockExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var statements = syn.Statements.Select(BuildStatement).ToFixedList();
            return new BlockExpression(syn.Span, type, semantics, statements);
        }

        private static IBoolLiteralExpression BuildBoolLiteralExpression(IBoolLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var value = syn.Value;
            return new BoolLiteralExpression(syn.Span, type, semantics, value);
        }

        private static IBorrowExpression BuildBorrowExpression(IBorrowExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var referent = BuildExpression(syn.Referent);
            return new BorrowExpression(syn.Span, type, semantics, referencedSymbol, referent);
        }

        private static IBreakExpression BuildBreakExpression(IBreakExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var value = BuildExpression(syn.Value);
            return new BreakExpression(syn.Span, type, semantics, value);
        }

        private static IFieldAccessExpression BuildFieldAccessExpression(IFieldAccessExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var context = BuildExpression(syn.Context);
            var accessOperator = syn.AccessOperator;
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            return new FieldAccessExpression(syn.Span, type, semantics, context, accessOperator, referencedSymbol);
        }

        private static IForeachExpression BuildForeachExpression(IForeachExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var symbol = syn.Symbol.Result;
            var inExpression = BuildExpression(syn.InExpression);
            var block = BuildBlockExpression(syn.Block);
            return new ForeachExpression(syn.Span, type, semantics, symbol, inExpression, block);
        }

        private static IFunctionInvocationExpression BuildFunctionInvocationExpression(IFunctionInvocationExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            FixedList<IExpression> arguments = syn.Arguments.Select(a => BuildExpression(a.Expression)).ToFixedList();
            return new FunctionInvocationExpression(syn.Span, type, semantics, referencedSymbol, arguments);
        }

        private static IIfExpression BuildIfExpression(IIfExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var condition = BuildExpression(syn.Condition);
            var thenBlock = BuildBlockOrResult(syn.ThenBlock);
            var elseClause = BuildElseClause(syn.ElseClause);
            return new IfExpression(syn.Span, type, semantics, condition, thenBlock, elseClause);
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
            var semantics = syn.Semantics.Assigned();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitImmutabilityConversion(syn.Span, type, semantics, expression, convertToType);
        }

        private static IImplicitNoneConversionExpression BuildImplicitNoneConversionExpression(IImplicitNoneConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitNoneConversionExpression(syn.Span, type, semantics, expression, convertToType);
        }

        private static IImplicitNumericConversionExpression BuildImplicitNumericConversionExpression(IImplicitNumericConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitNumericConversionExpression(syn.Span, type, semantics, expression, convertToType);
        }

        private static IImplicitOptionalConversionExpression BuildImplicitOptionalConversionExpression(IImplicitOptionalConversionExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var expression = BuildExpression(syn.Expression);
            var convertToType = syn.ConvertToType;
            return new ImplicitOptionalConversionExpression(syn.Span, type, semantics, expression, convertToType);
        }

        private static IIntegerLiteralExpression BuildIntegerLiteralExpression(IIntegerLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var value = syn.Value;
            return new IntegerLiteralExpression(syn.Span, type, semantics, value);
        }

        private static INoneLiteralExpression BuildNoneLiteralExpression(INoneLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            return new NoneLiteralExpression(syn.Span, type, semantics);
        }

        private static IStringLiteralExpression BuildStringLiteralExpression(IStringLiteralExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var value = syn.Value;
            return new StringLiteralExpression(syn.Span, type, semantics, value);
        }

        private static ILoopExpression BuildLoopExpression(ILoopExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var block = BuildBlockExpression(syn.Block);
            return new LoopExpression(syn.Span, type, semantics, block);
        }

        private static IMethodInvocationExpression BuildMethodInvocationExpression(IMethodInvocationExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var context = BuildExpression(syn.Context);
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            FixedList<IExpression> arguments = syn.Arguments.Select(a => BuildExpression(a.Expression)).ToFixedList();
            return new MethodInvocationExpression(syn.Span, type, semantics, context, referencedSymbol, arguments);
        }

        private static IMoveExpression BuildMoveExpression(IMoveExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var referent = BuildExpression(syn.Referent);
            return new MoveExpression(syn.Span, type, semantics, referencedSymbol, referent);
        }

        private static INameExpression BuildNameExpression(INameExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            return new NameExpression(syn.Span, type, semantics, referencedSymbol);
        }

        private static INewObjectExpression BuildNewObjectExpression(INewObjectExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            FixedList<IExpression> arguments = syn.Arguments.Select(a => BuildExpression(a.Expression)).ToFixedList();
            return new NewObjectExpression(syn.Span, type, semantics, referencedSymbol, arguments);
        }

        private static INextExpression BuildNextExpression(INextExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            return new NextExpression(syn.Span, type, semantics);
        }

        private static IReturnExpression BuildReturnExpression(IReturnExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var value = BuildExpression(syn.Value);
            return new ReturnExpression(syn.Span, type, semantics, value);
        }

        private static ISelfExpression BuildSelfExpression(ISelfExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var isImplicit = syn.IsImplicit;
            return new SelfExpression(syn.Span, type, semantics, referencedSymbol, isImplicit);
        }

        private static IShareExpression BuildShareExpression(IShareExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            var referent = BuildExpression(syn.Referent);
            return new ShareExpression(syn.Span, type, semantics, referencedSymbol, referent);
        }

        private static IUnaryOperatorExpression BuildUnaryOperatorExpression(IUnaryOperatorExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var fixity = syn.Fixity;
            var @operator = syn.Operator;
            var operand = BuildExpression(syn.Operand);
            return new UnaryOperatorExpression(syn.Span, type, semantics, fixity, @operator, operand);
        }

        private static IUnsafeExpression BuildUnsafeExpression(IUnsafeExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var expression = BuildExpression(syn.Expression);
            return new UnsafeExpression(syn.Span, type, semantics, expression);
        }

        private static IWhileExpression BuildWhileExpression(IWhileExpressionSyntax syn)
        {
            var type = syn.DataType ?? throw new InvalidOperationException();
            var semantics = syn.Semantics.Assigned();
            var condition = BuildExpression(syn.Condition);
            var block = BuildBlockExpression(syn.Block);
            return new WhileExpression(syn.Span, type, semantics, condition, block);
        }
    }
}
