//using Adamant.Tools.Compiler.Bootstrap.AST;
//using Adamant.Tools.Compiler.Bootstrap.Framework;
//using Adamant.Tools.Compiler.Bootstrap.Names;
//using JetBrains.Annotations;

//namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
//{
//    public class NameBuilder
//    {
//        [NotNull]
//        public Name BuildName([NotNull] NameSyntax nameSyntax)
//        {
//            switch (nameSyntax)
//            {
//                case QualifiedNameSyntax qualifiedNameSyntax:
//                {
//                    var qualifier = BuildName(qualifiedNameSyntax.Qualifier);
//                    var name = qualifiedNameSyntax.Name.Name;
//                    return qualifier.Qualify(name);
//                }
//                case IdentifierNameSyntax identifierNameSyntax:
//                {
//                    return identifierNameSyntax.Name;
//                }
//                default:
//                    throw NonExhaustiveMatchException.For(nameSyntax);
//            }
//        }
//    }
//}
