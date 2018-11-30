using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class DeclarationVisitor<A> : ExpressionVisitor<A>
    {
        public virtual void VisitDeclarations(
            [NotNull] IEnumerable<DeclarationSyntax> declarations,
            A args)
        {
            foreach (var declaration in declarations) VisitDeclaration(declaration, args);
        }

        public virtual void VisitDeclaration([CanBeNull] DeclarationSyntax declaration, A args)
        {
            switch (declaration)
            {
                case MemberDeclarationSyntax memberDeclaration:
                    VisitMemberDeclaration(memberDeclaration, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        public virtual void VisitMemberDeclaration([CanBeNull] MemberDeclarationSyntax memberDeclaration, A args)
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

        public virtual void VisitTypeDeclaration([CanBeNull] TypeDeclarationSyntax typeDeclaration, A args)
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

        public virtual void VisitClassDeclaration([NotNull] ClassDeclarationSyntax classDeclaration, A args)
        {
            foreach (var member in classDeclaration.Members)
                VisitDeclaration(member, args);
        }

        public virtual void VisitFunctionDeclaration([CanBeNull] FunctionDeclarationSyntax functionDeclaration, A args)
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

        public virtual void VisitConstructorDeclaration([NotNull] ConstructorDeclarationSyntax constructorDeclaration, A args)
        {
            if (constructorDeclaration.GenericParameters != null)
                foreach (var genericParameter in constructorDeclaration.GenericParameters)
                    VisitGenericParameter(genericParameter, args);

            foreach (var parameter in constructorDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitExpression(constructorDeclaration.Body, args);
        }

        public virtual void VisitNamedFunctionDeclaration([NotNull] NamedFunctionDeclarationSyntax namedFunctionDeclaration, A args)
        {
            if (namedFunctionDeclaration.GenericParameters != null)
                foreach (var genericParameter in namedFunctionDeclaration.GenericParameters)
                    VisitGenericParameter(genericParameter, args);

            foreach (var parameter in namedFunctionDeclaration.Parameters)
                VisitParameter(parameter, args);

            VisitExpression(namedFunctionDeclaration.ReturnTypeExpression, args);
            VisitExpression(namedFunctionDeclaration.Body, args);
        }

        public virtual void VisitParameter([NotNull] ParameterSyntax parameter, A args)
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

        public virtual void VisitFieldParameter([NotNull] FieldParameterSyntax fieldParameter, A args)
        {
            VisitExpression(fieldParameter.DefaultValue, args);
        }

        public virtual void VisitSelfParameter(SelfParameterSyntax selfParameter, A args)
        {
        }

        public virtual void VisitNamedParameter([NotNull] NamedParameterSyntax namedParameter, A args)
        {
            VisitExpression(namedParameter.TypeExpression, args);
            VisitExpression(namedParameter.DefaultValue, args);
        }

        public virtual void VisitGenericParameter([NotNull] GenericParameterSyntax genericParameter, A args)
        {
            VisitExpression(genericParameter.TypeExpression, args);
        }
    }
}
