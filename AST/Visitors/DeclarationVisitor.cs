using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class DeclarationVisitor<A> : ExpressionVisitor<A>
    {
        public void VisitDeclaration(IDeclarationSyntax? declaration, A args)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case null:
                    // Ignore
                    break;
                case IMemberDeclarationSyntax memberDeclaration:
                    VisitMemberDeclaration(memberDeclaration, args);
                    break;
                case INamespaceDeclarationSyntax namespaceDeclaration:
                    VisitNamespaceDeclaration(namespaceDeclaration, args);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    VisitClassDeclaration(classDeclaration, args);
                    break;
                case IFunctionDeclarationSyntax function:
                    VisitFunctionDeclaration(function, args);
                    break;
            }
        }

        public virtual void VisitFunctionDeclaration(IFunctionDeclarationSyntax function, A args)
        {
            foreach (var parameter in function.Parameters)
                VisitParameter(parameter, args);

            VisitType(function.ReturnTypeSyntax, args);
            if (function.Body != null)
                foreach (var statement in function.Body.Statements)
                    VisitStatement(statement, args);
        }

        public virtual void VisitNamespaceDeclaration(INamespaceDeclarationSyntax namespaceDeclaration, A args)
        {
            foreach (var usingDirective in namespaceDeclaration.UsingDirectives)
                VisitUsingDirective(usingDirective, args);

            foreach (var declaration in namespaceDeclaration.Declarations)
                VisitDeclaration(declaration, args);
        }

        public virtual void VisitUsingDirective(IUsingDirectiveSyntax usingDirective, A args)
        {
        }

        public void VisitMemberDeclaration(IMemberDeclarationSyntax? memberDeclaration, A args)
        {
            switch (memberDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(memberDeclaration);
                case null:
                    // Ignore
                    break;
                case IMethodDeclarationSyntax functionDeclaration:
                    VisitMethodDeclaration(functionDeclaration, args);
                    break;
                case IFieldDeclarationSyntax fieldDeclaration:
                    VisitFieldDeclaration(fieldDeclaration, args);
                    break;
                case IConstructorDeclarationSyntax constructor:
                    VisitConstructorDeclaration(constructor, args);
                    break;
            }
        }

        public virtual void VisitFieldDeclaration(IFieldDeclarationSyntax fieldDeclaration, A args)
        {
            VisitType(fieldDeclaration.TypeSyntax, args);
            VisitExpression(fieldDeclaration.Initializer, args);
        }

        public virtual void VisitClassDeclaration(IClassDeclarationSyntax classDeclaration, A args)
        {
            foreach (var member in classDeclaration.Members)
                VisitDeclaration(member, args);
        }

        public virtual void VisitConstructorDeclaration(IConstructorDeclarationSyntax constructorDeclaration, A args)
        {
            foreach (var parameter in constructorDeclaration.Parameters)
                VisitParameter(parameter, args);

            foreach (var statement in constructorDeclaration.Body.Statements)
                VisitStatement(statement, args);
        }

        public virtual void VisitMethodDeclaration(IMethodDeclarationSyntax? methodDeclaration, A args)
        {
            switch (methodDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(methodDeclaration);
                case null:
                    // Ignore
                    break;
                case IConcreteMethodDeclarationSyntax concreteMethod:
                    VisitConcreteMethodDeclaration(concreteMethod, args);
                    break;
                case IAbstractMethodDeclarationSyntax abstractMethod:
                    VisitAbstractMethodDeclaration(abstractMethod, args);
                    break;
            }
        }

        private void VisitAbstractMethodDeclaration(IAbstractMethodDeclarationSyntax abstractMethodDeclaration, A args)
        {
            foreach (var parameter in abstractMethodDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitType(abstractMethodDeclaration.ReturnTypeSyntax, args);
        }

        public virtual void VisitConcreteMethodDeclaration(IConcreteMethodDeclarationSyntax concreteMethodDeclaration, A args)
        {
            foreach (var parameter in concreteMethodDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitType(concreteMethodDeclaration.ReturnTypeSyntax, args);
            foreach (var statement in concreteMethodDeclaration.Body.Statements)
                VisitStatement(statement, args);
        }

        public virtual void VisitParameter(IParameterSyntax? parameter, A args)
        {
            switch (parameter)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter);
                case null:
                    // Ignore
                    break;
                case INamedParameterSyntax namedParameter:
                    VisitNamedParameter(namedParameter, args);
                    break;
                case ISelfParameterSyntax selfParameter:
                    VisitSelfParameter(selfParameter, args);
                    break;
                case IFieldParameterSyntax fieldParameter:
                    VisitFieldParameter(fieldParameter, args);
                    break;
            }
        }

        public virtual void VisitFieldParameter(IFieldParameterSyntax fieldParameter, A args)
        {
            VisitExpression(fieldParameter.DefaultValue, args);
        }

        public virtual void VisitSelfParameter(ISelfParameterSyntax selfParameter, A args)
        {
        }

        public virtual void VisitNamedParameter(INamedParameterSyntax namedParameter, A args)
        {
            VisitType(namedParameter.TypeSyntax, args);
            VisitExpression(namedParameter.DefaultValue, args);
        }
    }
}
