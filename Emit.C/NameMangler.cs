using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
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
        // try https://github.com/atifaziz/Nunycode
        readonly IdnMapping mapping = new IdnMapping();

        // Note, we don't have to worry about whether the identifier starts with
        // a number because it will end up prefixed anyway.
        private static readonly Regex StandardIdentifierPattern = new Regex(@"^[_0-9a-zA-Z]+$", RegexOptions.Compiled);

        public string MangleName(FunctionDeclaration function)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(function.FullName) + 5);
            Mangle(function.FullName, builder);
            builder.Append("__");
            builder.Append(function.Arity);
            return mapping.GetAscii(builder.ToString());
        }
        public string MangleUnqualifiedName(FunctionDeclaration function)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(function.FullName.UnqualifiedName) + 5);
            Mangle(function.FullName.UnqualifiedName, builder);
            builder.Append("__");
            builder.Append(function.Arity);
            return mapping.GetAscii(builder.ToString());
        }

        public string MangleName(ConstructorDeclaration constructor)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(constructor.FullName) + 5);
            Mangle(constructor.FullName, builder);
            builder.Append("__");
            builder.Append(constructor.Arity);
            return mapping.GetAscii(builder.ToString());
        }

        public string MangleName(ClassDeclaration @class)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(@class.FullName) + 5);
            Mangle(@class.FullName, builder);
            //if (type.IsGeneric)
            //{
            //    builder.Append("__");
            //    builder.Append(type.GenericArity);
            //}
            return mapping.GetAscii(builder.ToString());
        }

        public object Mangle(DataType type)
        {
            switch (type)
            {
                default:
                    throw new NotImplementedException();
                //throw ExhaustiveMatch.Failed(type);
                case SimpleType simpleType:
                    return Mangle(simpleType);
                case UserObjectType userObjectType:
                    return Mangle(userObjectType);
            }
        }

        public string Mangle(SimpleType type)
        {
            return Mangle(type.Name);
        }

        public string Mangle(UserObjectType type)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(type.Name) + 5);
            Mangle(type.Name, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public string Mangle(Name name)
        {
            var builder = new StringBuilder(EstimateSize(name));
            Mangle(name, builder);
            return mapping.GetAscii(builder.ToString());
        }

        public string Mangle(string name)
        {
            return Mangle(new SimpleName(name));
        }

        private static int EstimateSize(Name name)
        {
            switch (name)
            {
                default:
                    throw ExhaustiveMatch.Failed(name);
                case QualifiedName qualifiedName:
                    return EstimateSize(qualifiedName.Qualifier) + 2 +
                           EstimateSize(qualifiedName.UnqualifiedName);
                case SimpleName simpleName:
                    return simpleName.Text.Length;
            }
        }

        private static void Mangle(Name name, StringBuilder builder)
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
                    builder.Append("__");
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
