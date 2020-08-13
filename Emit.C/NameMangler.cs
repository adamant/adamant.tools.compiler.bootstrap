using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    /// <summary>
    /// This new name mangling scheme was adopted because the old scheme was hard
    /// to read when output to the console. clang would not correctly output the
    /// Unicode characters in it. The new scheme doesn't guarantee uniqueness,
    /// but should be good enough until we switch to LLVM.
    ///
    /// * replace " " with "_"
    /// * use "__" to separate segments
    /// * use "__" and a number for arity
    /// * special names are prefixed with "_" because names don't normally start with underscore
    /// * encode with punycode
    ///
    /// </summary>
    public class NameMangler
    {
        private const string Separator = "__";

        // try https://github.com/atifaziz/Nunycode
        private readonly IdnMapping mapping = new IdnMapping();

        // Note, we don't have to worry about whether the identifier starts with
        // a number because it will end up prefixed anyway.
        private static readonly Regex StandardIdentifierPattern = new Regex(@"^[_0-9a-zA-Z]+$", RegexOptions.Compiled);

        public string SelfName { get; } = "_self";

        public string MangleName(FunctionDeclaration function)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(function.FullName) + 5);
            Mangle(function.FullName, builder);
            builder.Append(Separator);
            builder.Append(function.Arity);
            return mapping.GetAscii(builder.ToString());
        }

        public string MangleName(MethodDeclaration method)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(method.FullName) + 5);
            Mangle(method.FullName, builder);
            builder.Append(Separator);
            builder.Append(method.Arity + 1); // add one for self parameter
            return mapping.GetAscii(builder.ToString());
        }
        public string MangleUnqualifiedName(MethodDeclaration method)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(method.FullName.UnqualifiedName) + 5);
            Mangle(method.FullName.UnqualifiedName, builder);
            builder.Append(Separator);
            builder.Append(method.Arity + 1); // add one for self parameter
            return mapping.GetAscii(builder.ToString());
        }

        public string MangleName(ConstructorDeclaration constructor)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(constructor.FullName) + 5);
            Mangle(constructor.FullName, builder);
            builder.Append(Separator);
            builder.Append(constructor.Arity);
            return mapping.GetAscii(builder.ToString());
        }

        public string MangleName(ClassDeclaration @class)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(@class.FullName) + 5);
            Mangle(@class.FullName, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public object Mangle(DataType type)
        {
            return type switch
            {
                SimpleType simpleType => Mangle(simpleType),
                ObjectType objectType => Mangle(objectType),
                _ => throw new NotImplementedException(),
                //_ => throw ExhaustiveMatch.Failed(type),
            };
        }

        public string Mangle(SimpleType type)
        {
            return Mangle(type.Name);
        }

        public string Mangle(ObjectType type)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateNamespaceSize(type.ContainingNamespace) + EstimateSize(type.Name) + 5);
            MangleNamespace(type.ContainingNamespace, builder);
            Mangle(type.Name, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public string Mangle(MaybeQualifiedName name)
        {
            var builder = new StringBuilder(EstimateSize(name));
            Mangle(name, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public string Mangle(TypeName name)
        {
            var builder = new StringBuilder(EstimateSize(name));
            Mangle(name, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public string Mangle(Symbol symbol)
        {
            var builder = new StringBuilder(EstimateSize(symbol.ContainingSymbol));
            Mangle(symbol, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public string MangleMethod(MethodSymbol symbol)
        {
            var builder = new StringBuilder(EstimateSize(symbol.ContainingSymbol));
            MangleMethod(symbol, builder);
            return mapping.GetAscii(builder.ToString());
        }

        private static int EstimateNamespaceSize(NamespaceName namespaceName)
        {
            return namespaceName.Segments.Sum(s => s.Text.Length + 2);
        }

        private static int EstimateSize(TypeName? typeName)
        {
            return typeName switch
            {
                null => 0,
                SpecialTypeName specialName => 1 + specialName.Text.Length,
                Name name => name.Text.Length,
                _ => throw ExhaustiveMatch.Failed(typeName)
            };
        }

        private static int EstimateSize(Symbol? symbol)
        {
            if (symbol is null || symbol is PackageSymbol) return 0;
            return EstimateSize(symbol.ContainingSymbol) + 2 + EstimateSize(symbol.Name);
        }

        private static int EstimateSize(MaybeQualifiedName name)
        {
            return name switch
            {
                QualifiedName qualifiedName => EstimateSize(qualifiedName.Qualifier) + 2
                                               + EstimateSize(qualifiedName.UnqualifiedName),
                SimpleName simpleName => simpleName.Text.Length,
                _ => throw ExhaustiveMatch.Failed(name)
            };
        }

        private static void MangleNamespace(NamespaceName namespaceName, StringBuilder builder)
        {
            foreach (var name in namespaceName.Segments)
            {
                builder.Append(name.Text);
                builder.Append(Separator);
            }
        }

        private static void Mangle(TypeName typeName, StringBuilder builder)
        {
            switch (typeName)
            {
                default:
                    throw ExhaustiveMatch.Failed(typeName);
                case Name name:
                    ManglePart(name.Text, builder);
                    break;
                case SpecialTypeName specialName:
                    builder.Append('_');
                    ManglePart(specialName.Text, builder);
                    break;
            }
        }

        private static void Mangle(Symbol? symbol, StringBuilder builder)
        {
            switch (symbol)
            {
                default:
                    throw new NotImplementedException(symbol.GetType().Name);
                case null:
                    // Nothing
                    break;
                case NamespaceSymbol sym:
                    Mangle(sym.ContainingSymbol, builder);
                    AppendSeparator(sym.ContainingSymbol, builder);
                    Mangle(sym.Name, builder);
                    break;
                case PackageSymbol _:
                    // Nothing
                    break;
                case FunctionSymbol sym:
                    Mangle(sym.ContainingSymbol, builder);
                    AppendSeparator(sym.ContainingSymbol, builder);
                    Mangle(sym.Name, builder);
                    builder.Append(Separator);
                    builder.Append(sym.Arity);
                    break;
                case ObjectTypeSymbol sym:
                    Mangle(sym.ContainingSymbol, builder);
                    AppendSeparator(sym.ContainingSymbol, builder);
                    Mangle(sym.Name, builder);
                    break;
                case MethodSymbol sym:
                    Mangle(sym.ContainingSymbol, builder);
                    builder.Append(Separator);
                    MangleMethod(sym, builder);
                    break;
                case PrimitiveTypeSymbol sym:
                    Mangle(sym.ContainingSymbol, builder);
                    AppendSeparator(sym.ContainingSymbol, builder);
                    Mangle(sym.Name, builder);
                    break;
                case ConstructorSymbol sym:
                    Mangle(sym.ContainingSymbol, builder);
                    builder.Append(Separator);
                    builder.Append("_new");
                    if (!(sym.Name is null))
                    {
                        builder.Append("_");
                        Mangle(sym.Name, builder);
                    }
                    builder.Append(Separator);
                    builder.Append(sym.Arity + 1); // add one for self parameter
                    break;
            }
        }

        private static void MangleMethod(MethodSymbol symbol, StringBuilder builder)
        {
            Mangle(symbol.Name, builder);
            builder.Append(Separator);
            builder.Append(symbol.Arity + 1); // add one for self parameter
        }

        private static void AppendSeparator(Symbol? symbol, StringBuilder builder)
        {
            if (symbol is null || symbol is PackageSymbol) return;
            builder.Append(Separator);
        }

        private static void Mangle(MaybeQualifiedName name, StringBuilder builder)
        {
            switch (name)
            {
                default:
                    throw ExhaustiveMatch.Failed(name);
                case SimpleName simpleName:
                    if (simpleName.IsSpecial)
                        builder.Append('_');
                    ManglePart(simpleName.Text, builder);
                    break;
                case QualifiedName qualifiedName:
                    Mangle(qualifiedName.Qualifier, builder);
                    builder.Append(Separator);
                    Mangle(qualifiedName.UnqualifiedName, builder);
                    break;
            }
        }

        internal static void ManglePart(string name, StringBuilder builder)
        {
            // Fast path no need to escape anything
            if (StandardIdentifierPattern.IsMatch(name))
            {
                builder.Append(name);
                return;
            }

            builder.Append(name.Replace(' ', '_'));
        }
    }
}
