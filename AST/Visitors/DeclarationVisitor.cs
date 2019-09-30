using System.Collections.Generic;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class DeclarationVisitor<A> : ExpressionVisitor<A>
    {
        public virtual void VisitDeclarations(IEnumerable<IDeclarationSyntax> declarations, A args)
        {
            foreach (var declaration in declarations)
                VisitDeclaration(declaration, args);
        }

        public virtual void VisitDeclaration(IDeclarationSyntax declaration, A args)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IMemberDeclarationSyntax memberDeclaration:
                    VisitMemberDeclaration(memberDeclaration, args);
                    break;
                case INamespaceDeclarationSyntax namespaceDeclaration:
                    VisitNamespaceDeclaration(namespaceDeclaration, args);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    VisitClassDeclaration(classDeclaration, args);
                    break;
                case null:
                    // Ignore
                    break;
            }
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

        public virtual void VisitMemberDeclaration(IMemberDeclarationSyntax memberDeclaration, A args)
        {
            switch (memberDeclaration)
            {
                case IFunctionDeclarationSyntax functionDeclaration:
                    VisitFunctionDeclaration(functionDeclaration, args);
                    break;
                case IFieldDeclarationSyntax fieldDeclaration:
                    VisitFieldDeclaration(fieldDeclaration, args);
                    break;
                case IConstructorDeclarationSyntax constructor:
                    VisitConstructorDeclaration(constructor, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw ExhaustiveMatch.Failed(memberDeclaration);
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

            foreach (var statement in constructorDeclaration.Body)
                VisitStatement(statement, args);
        }

        public virtual void VisitFunctionDeclaration(IFunctionDeclarationSyntax functionDeclaration, A args)
        {
            foreach (var parameter in functionDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitType(functionDeclaration.ReturnTypeSyntax, args);
            if (functionDeclaration.Body != null)
                foreach (var statement in functionDeclaration.Body)
                    VisitStatement(statement, args);
        }

        public virtual void VisitParameter(IParameterSyntax parameter, A args)
        {
            switch (parameter)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter);
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
