using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
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
    /// `ₓ` U+2093 prefixes all keyword/built in names. May occur in the middle
    ///             of a fully qualified name, for example `My_Class·ₓnew`
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

        public string FunctionName([NotNull] FunctionDeclaration function)
        {
            return "";
            //var builder = new StringBuilder(UnderscoreRuns.Replace(function.Name, "_$0"));
            //builder.Append("__");
            //builder.Append(function.Parameters.Count);
            //return builder.ToString();
        }

        // TODO test this
        public string NamePart([NotNull] string part)
        {
            // Fast path no need to escape anything
            if (StandardIdentifierPattern.IsMatch(part))
                return part;

            // We separate the handling of characters in the Basic Multilingual
            // Plan (BMP) from those outside it
            var builder = new StringBuilder();
            for (var i = 0; i < part.Length; i++)
            {
                var c = part[i];
                int codePoint;
                if (char.IsHighSurrogate(c))
                {
                    var high = c;
                    var low = part[i + 1];
                    codePoint = char.ConvertToUtf32(high, low);
                    if (IsValidSupplementaryPlaneIdentifierCharacter(codePoint))
                    {
                        builder.Append(high);
                        builder.Append(low);
                        break;
                    }
                }
                else
                {
                    if (c == ' ')
                    {
                        builder.Append('˽');
                        break;
                    }
                    if (IsValidBasicMultilingualPlaneIdentifierCharacter(c)
                        && !IsEscapeCharacter(c))
                    {
                        builder.Append(c);
                        break;
                    }

                    codePoint = c;
                }

                builder.Append('µ');
                builder.Append(codePoint.ToString("X")); // Supposedly faster than using AppendFormat()
                builder.Append('ǂ');
            }

            return builder.ToString();
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
            return c == 'ᵢ' || c == 'ₓ' || c == '˽' || c == '·' || c == '´' || c == 'µ' || c == 'ǂ';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSupplementaryPlaneIdentifierCharacter(int c)
        {
            // C Spec Annex D
            // Line 8
            //10000−1FFFD, 20000−2FFFD, 30000−3FFFD, 40000−4FFFD, 50000−5FFFD,
            //60000−6FFFD, 70000−7FFFD, 80000−8FFFD, 90000−9FFFD, A0000−AFFFD,
            //B0000−BFFFD, C0000−CFFFD, D0000−DFFFD, E0000−EFFFD
            throw new NotImplementedException();
        }
    }
}
