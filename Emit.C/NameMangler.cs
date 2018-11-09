using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    /// <summary>
    /// LLVM supports placing any character in an identifier. This means that
    /// fully qualified names can be used directly as identifiers and an escape
    /// sequence scheme will only be needed for those characters that occur in
    /// fully qualified names anyway. With that in mind, it doesn't make sense to
    /// come up with too elaborate of a name mangling scheme. Ideally, we want
    /// the names to be readable too.
    ///
    /// C now allows quite a few unicode characters in identifiers now. Given
    /// that, it seems best to translate most names as is. However, certain
    /// characters will appear very frequently so it makes sense to give them
    /// special representations.
    ///
    /// `ᵢ` U+1D62 prefixes all user identifiers to avoid name conflicts and issues
    ///             with what is a legal identifier start character.
    ///             Abbreviation for identifier. Occurs only once at the beginning.
    /// `ₐ` U+2090 prefixes all keyword/built in names. May occur in the middle
    ///             of a fully qualified name, for example `My_Class·ₐnew`.
    ///             'a' is for "Adamant"
    /// `˽` U+02FD replaces all spaces
    /// `·` U+00B7 is used to separate name parts, as `myNamespace·My_Class`
    /// `´` U+00B4 separates arity from the name
    /// `µ`, `ǂ` U+00B5, U+01C2 All characters in the original identifier not
    ///             valid in a C identifier or matching one of these characters
    ///             are encoded by the micro sign followed by the codepoint in
    ///             hexadecimal terminated by the click symbol `ǂ`.
    ///
    /// If needed subscript parens `₍` U+207D and `₎` U+207E are available for
    /// bracketing generic arguments.
    ///
    /// Note: this name mangling explicitly does not support having the same
    ///         fully qualified name defined in multiple packages because it
    ///         does not include the package name as part of the mangled name.
    ///
    /// Note: many of these characters can be found at:
    /// * https://en.wikipedia.org/wiki/Unicode_subscripts_and_superscripts
    /// * https://en.wikipedia.org/wiki/List_of_Unicode_characters
    /// </summary>
    public class NameMangler
    {
        // Note, we don't have to worry about whether the identifier starts with
        // a number because it will end up prefixed anyway.
        [NotNull]
        private static readonly Regex StandardIdentifierPattern = new Regex(@"^[_0-9a-zA-Z]+$", RegexOptions.Compiled);

        [NotNull]
        public string MangleName([NotNull] FunctionDeclaration function)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(function.Name) + 5);
            Mangle(function.Name, builder);
            builder.Append('´');
            builder.Append(function.Arity);
            return builder.ToString().AssertNotNull();
        }

        [NotNull]
        public string MangleName([NotNull] TypeDeclaration type)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(type.Name) + 2);
            Mangle(type.Name, builder);
            if (type.IsGeneric)
            {
                builder.Append('´');
                builder.Append(type.GenericArity);
            }
            return builder.ToString().AssertNotNull();
        }

        [NotNull]
        public string Mangle([NotNull] ObjectType type)
        {
            // builder with room for the characters we are likely to add
            var builder = new StringBuilder(EstimateSize(type.Name) + 2);
            Mangle(type.Name, builder);
            if (type.IsGeneric)
            {
                builder.Append('´');
                builder.Append(type.GenericArity);
            }
            return builder.ToString().AssertNotNull();
        }

        [NotNull]
        public string Mangle([NotNull] Name name)
        {
            var builder = new StringBuilder(EstimateSize(name));
            Mangle(name, builder);
            return builder.ToString().AssertNotNull();
        }

        [NotNull]
        public string Mangle([NotNull] string name)
        {
            return Mangle(new SimpleName(name));
        }

        private static int EstimateSize([NotNull] Name name)
        {
            switch (name)
            {
                case QualifiedName qualifiedName:
                    return EstimateSize(qualifiedName.Qualifier) + 1 +
                           EstimateSize(qualifiedName.UnqualifiedName);
                case SimpleName simpleName:
                    return 1 + simpleName.Text.Length;
                default:
                    throw NonExhaustiveMatchException.For(name);
            }
        }

        private static void Mangle([NotNull] Name name, [NotNull] StringBuilder builder)
        {
            switch (name)
            {
                case SimpleName simpleName:
                    builder.Append(simpleName.IsSpecial ? 'ₐ' : 'ᵢ');
                    ManglePart(simpleName.Text, builder);
                    break;
                case QualifiedName qualifiedName:
                    Mangle(qualifiedName.Qualifier, builder);
                    builder.Append('·');
                    Mangle(qualifiedName.UnqualifiedName, builder);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(name);
            }
        }

        /// <summary>
        /// Mangle an individual part of a name.
        /// </summary>
        [NotNull]
        internal static void ManglePart([NotNull] string name, [NotNull] StringBuilder builder)
        {
            // Fast path no need to escape anything
            if (StandardIdentifierPattern.IsMatch(name))
            {
                builder.Append(name);
                return;
            }

            // We separate the handling of characters in the Basic Multilingual
            // Plan (BMP) from those outside it
            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];
                int codePoint;
                // If a high surrogate ends the string, we just encode it
                if (char.IsHighSurrogate(c) && i + 1 < name.Length)
                {
                    i += 1; // we move on to the low surrogate
                    var high = c;
                    var low = name[i];
                    codePoint = char.ConvertToUtf32(high, low);
                    if (IsValidSupplementaryPlaneIdentifierCharacter(codePoint))
                    {
                        builder.Append(high);
                        builder.Append(low);
                        continue;
                    }
                }
                else
                {
                    if (c == ' ')
                    {
                        builder.Append('˽');
                        continue;
                    }
                    if (IsValidBasicMultilingualPlaneIdentifierCharacter(c)
                        && !IsEscapeCharacter(c))
                    {
                        builder.Append(c);
                        continue;
                    }

                    codePoint = c;
                }

                builder.Append('µ');
                builder.Append(codePoint.ToString("X")); // Supposedly faster than using AppendFormat()
                builder.Append('ǂ');
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidBasicMultilingualPlaneIdentifierCharacter(char c)
        {
            return c == '_'
                || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
                // C Spec Annex D
                // Line 1
                || c == 0xA8 || c == 0xAA || c == 0xAD || c == 0xAF || (c >= 0xB2 && c <= 0xB5)
                || (c >= 0xB7 && c <= 0xBA) || (c >= 0xBC && c <= 0xBE) || (c >= 0xC0 && c <= 0xD6)
                || (c >= 0xD8 && c <= 0xF6) || (c >= 0xF8 && c <= 0xFF)
                // Line 2
                || (c >= 0x0100 && c <= 0x167F) || (c >= 0x1681 && c <= 0x180D) || (c >= 0x180F && c <= 0x1FFF)
                // Line 3
                || (c >= 0x200B && c <= 0x200D) || (c >= 0x202A && c <= 0x202E) || (c >= 0x203F && c <= 0x2040)
                || c == 0x2054 || (c >= 0x2060 && c <= 0x206F)
                // Line 4
                || (c >= 0x2070 && c <= 0x218F) || (c >= 0x2460 && c <= 0x24FF) || (c >= 0x2776 && c <= 0x2793)
                || (c >= 0x2C00 && c <= 0x2DFF) || (c >= 0x2E80 && c <= 0x2FFF)
                // Line 5
                || (c >= 0x3004 && c <= 0x3007) || (c >= 0x3021 && c <= 0x302F) || (c >= 0x3031 && c <= 0x303F)
                // Line 6
                || (c >= 0x3040 && c <= 0xD7FF)
                // Line 7
                || (c >= 0xF900 && c <= 0xFD3D) || (c >= 0xFD40 && c <= 0xFDCF) || (c >= 0xFDF0 && c <= 0xFE44)
                || (c >= 0xFE47 && c <= 0xFFFD)
                ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEscapeCharacter(char c)
        {
            return c == 'ᵢ' || c == 'ₐ' || c == '˽' || c == '·' || c == '´' || c == 'µ' || c == 'ǂ';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSupplementaryPlaneIdentifierCharacter(int c)
        {
            return
                // C Spec Annex D
                // Line 8
                (c >= 0x10000 && c <= 0x1FFFD)
                || (c >= 0x20000 && c <= 0x2FFFD)
                || (c >= 0x30000 && c <= 0x3FFFD)
                || (c >= 0x40000 && c <= 0x4FFFD)
                || (c >= 0x50000 && c <= 0x5FFFD)
                || (c >= 0x60000 && c <= 0x6FFFD)
                || (c >= 0x70000 && c <= 0x7FFFD)
                || (c >= 0x80000 && c <= 0x8FFFD)
                || (c >= 0x90000 && c <= 0x9FFFD)
                || (c >= 0xA0000 && c <= 0xAFFFD)
                || (c >= 0xB0000 && c <= 0xBFFFD)
                || (c >= 0xC0000 && c <= 0xCFFFD)
                || (c >= 0xD0000 && c <= 0xDFFFD)
                || (c >= 0xE0000 && c <= 0xEFFFD)
                ;
        }
    }
}
