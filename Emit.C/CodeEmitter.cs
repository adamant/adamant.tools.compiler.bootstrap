using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Emit.C.Properties;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CodeEmitter : Emitter
    {
        [NotNull] private readonly NameMangler nameMangler = new NameMangler();

        [NotNull] public static string RuntimeLibraryCode => Resources.RuntimeLibraryCode.AssertNotNull();
        [NotNull] public const string RuntimeLibraryCodeFileName = "RuntimeLibrary.c";
        [NotNull] public static string RuntimeLibraryHeader => Resources.RuntimeLibraryHeader.AssertNotNull();
        [NotNull] public const string RuntimeLibraryHeaderFileName = "RuntimeLibrary.h";

        public override string Emit([NotNull] Package package)
        {
            var code = new Code();

            EmitPreamble(code);

            foreach (var declaration in package.Declarations)
                Emit(declaration, code);

            EmitEntryPointAdapter(package, code);

            EmitPostamble(code);

            return code.ToString();
        }

        private void EmitPreamble([NotNull] Code code)
        {
            // Setup the beginning of each section
            code.Includes.AppendLine($"#include \"{RuntimeLibraryHeaderFileName}\"");

            code.TypeIdDeclaration.AppendLine("// Type ID Declarations");
            // Type ID Enum
            //code.TypeIdDeclaration.AppendLine("enum Type_ID");
            //code.TypeIdDeclaration.BeginBlock();
            //code.TypeIdDeclaration.AppendLine("never__0__0TypeID = 0,");
            // TODO setup primitive types?

            code.TypeDeclarations.AppendLine("// Type Declarations");
            code.FunctionDeclarations.AppendLine("// Function Declarations");
            code.ClassDeclarations.AppendLine("// Class Declarations");
            code.GlobalDefinitions.AppendLine("// Global Definitions");
            code.Definitions.AppendLine("// Definitions");
        }

        private void Emit([NotNull] Declaration declaration, [NotNull]  Code code)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    Emit(function, code);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void Emit([NotNull] FunctionDeclaration function, [NotNull] Code code)
        {
            var name = nameMangler.MangleName(function);
            var parameters = Convert(function.Parameters);
            var returnType = Convert(function.ReturnType);

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            //Emit(code, function.Body);
            code.Definitions.BeginBlock();
            code.Definitions.EndBlock();
        }

        #region Convert
        private static string Convert(DataType type)
        {
            switch (type)
            {
                // TODO perhaps the name mangler should be used on primitives
                case var t when t == ObjectType.Void:
                    return "void";
                case var t when t == ObjectType.Int:
                    return "ₐint";
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }

        private string Convert([NotNull][ItemNotNull] IEnumerable<Parameter> parameters)
        {
            return string.Join(", ", parameters.Select(Convert));
        }

        private string Convert([NotNull] Parameter parameter)
        {
            throw new NotImplementedException();
            //if (parameter.MutableBinding)
            //    return $"{Convert(parameter.Type.Type)} {parameter.Name}";
            //else
            //    return $"const {Convert(parameter.Type.Type)} {parameter.Name}";
        }
        #endregion

        private void EmitEntryPointAdapter([NotNull] Package package, [NotNull] Code code)
        {
            if (package.EntryPoint == null) return;

            var entryPoint = package.EntryPoint;
            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine("// Entry Point Adapter");
            code.Definitions.AppendLine("int32_t main(const int argc, char const * const * const argv)");
            code.Definitions.BeginBlock();
            code.Definitions.AppendLine($"{nameMangler.MangleName(entryPoint)}();");
            code.Definitions.AppendLine("return 0;");
            code.Definitions.EndBlock();
        }

        private void EmitPostamble([NotNull] Code code)
        {
            // Close the Type_ID enum
            //code.TypeIdDeclaration.EndBlockWithSemicolon();
            //code.TypeIdDeclaration.AppendLine("typedef enum Type_ID Type_ID;");
        }
    }
}
