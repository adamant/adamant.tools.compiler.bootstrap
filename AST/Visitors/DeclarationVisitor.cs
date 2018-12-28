using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class DeclarationVisitor<A> : ExpressionVisitor<A>
    {
        public virtual void VisitDeclarations(
             IEnumerable<DeclarationSyntax> declarations,
            A args)
        {
            foreach (var declaration in declarations) VisitDeclaration(declaration, args);
        }

        public virtual void VisitDeclaration(DeclarationSyntax declaration, A args)
        {
            switch (declaration)
            {
                case MemberDeclarationSyntax memberDeclaration:
                    VisitMemberDeclaration(memberDeclaration, args);
                    break;
                case NamespaceDeclarationSyntax namespaceDeclaration:
                    VisitNamespaceDeclaration(namespaceDeclaration, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        public virtual void VisitNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclaration, A args)
        {
            foreach (var usingDirective in namespaceDeclaration.UsingDirectives)
                VisitUsingDirective(usingDirective, args);

            foreach (var declaration in namespaceDeclaration.Declarations)
                VisitDeclaration(declaration, args);
        }

        public virtual void VisitUsingDirective(UsingDirectiveSyntax usingDirective, A args)
        {
        }

        public virtual void VisitMemberDeclaration(MemberDeclarationSyntax memberDeclaration, A args)
        {
            switch (memberDeclaration)
            {
                case FunctionDeclarationSyntax functionDeclaration:
                    VisitFunctionDeclaration(functionDeclaration, args);
                    break;
                case TypeDeclarationSyntax typeDeclaration:
                    VisitTypeDeclaration(typeDeclaration, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(memberDeclaration);
            }
        }

        public virtual void VisitTypeDeclaration(TypeDeclarationSyntax typeDeclaration, A args)
        {
            switch (typeDeclaration)
            {
                case ClassDeclarationSyntax classDeclaration:
                    VisitClassDeclaration(classDeclaration, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(typeDeclaration);
            }
        }

        public virtual void VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, A args)
        {
            foreach (var member in classDeclaration.Members)
                VisitDeclaration(member, args);
        }

        public virtual void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, A args)
        {
            switch (functionDeclaration)
            {
                case NamedFunctionDeclarationSyntax namedFunction:
                    VisitNamedFunctionDeclaration(namedFunction, args);
                    break;
                case ConstructorDeclarationSyntax constructorDeclaration:
                    VisitConstructorDeclaration(constructorDeclaration, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(functionDeclaration);
            }
        }

        public virtual void VisitConstructorDeclaration(ConstructorDeclarationSyntax constructorDeclaration, A args)
        {
            if (constructorDeclaration.GenericParameters != null)
                foreach (var genericParameter in constructorDeclaration.GenericParameters)
                    VisitGenericParameter(genericParameter, args);

            foreach (var parameter in constructorDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitExpression(constructorDeclaration.Body, args);
        }

        public virtual void VisitNamedFunctionDeclaration(NamedFunctionDeclarationSyntax namedFunctionDeclaration, A args)
        {
            if (namedFunctionDeclaration.GenericParameters != null)
                foreach (var genericParameter in namedFunctionDeclaration.GenericParameters)
                    VisitGenericParameter(genericParameter, args);

            foreach (var parameter in namedFunctionDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitExpression(namedFunctionDeclaration.ReturnTypeExpression, args);
            VisitExpression(namedFunctionDeclaration.Body, args);
        }

        public virtual void VisitParameter(ParameterSyntax parameter, A args)
        {
            switch (parameter)
            {
                case NamedParameterSyntax namedParameter:
                    VisitNamedParameter(namedParameter, args);
                    break;
                case SelfParameterSyntax selfParameter:
                    VisitSelfParameter(selfParameter, args);
                    break;
                case FieldParameterSyntax fieldParameter:
                    VisitFieldParameter(fieldParameter, args);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }

        public virtual void VisitFieldParameter(FieldParameterSyntax fieldParameter, A args)
        {
            VisitExpression(fieldParameter.DefaultValue, args);
        }

        public virtual void VisitSelfParameter(SelfParameterSyntax selfParameter, A args)
        {
        }

        public virtual void VisitNamedParameter(NamedParameterSyntax namedParameter, A args)
        {
            VisitExpression(namedParameter.TypeExpression, args);
            VisitExpression(namedParameter.DefaultValue, args);
        }

        public virtual void VisitGenericParameter(GenericParameterSyntax genericParameter, A args)
        {
            VisitExpression(genericParameter.TypeExpression, args);
        }
    }
}
