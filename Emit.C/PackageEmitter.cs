using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class PackageEmitter : IEmitter<PackageIL>
    {
        private readonly NameMangler nameMangler;
        private readonly IEmitter<DeclarationIL> declarationEmitter;

        public PackageEmitter(
            NameMangler nameMangler,
            IEmitter<DeclarationIL> declarationEmitter)
        {
            this.nameMangler = nameMangler;
            this.declarationEmitter = declarationEmitter;
        }

        public void Emit(PackageIL package, Code code)
        {
            foreach (var declaration in package.Declarations)
                declarationEmitter.Emit(declaration, code);

            EmitEntryPointAdapter(package.EntryPoint, code);
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public void EmitPreamble(Code code)
        {
            // Setup the beginning of each section
            code.Includes.AppendLine($"#include \"{CodeEmitter.RuntimeLibraryHeaderFileName}\"");

            code.TypeIdDeclaration.AppendLine("// Type ID Declarations");
            // Type ID Enum
            code.TypeIdDeclaration.AppendLine("enum _Type_ID");
            code.TypeIdDeclaration.BeginBlock();
            code.TypeIdDeclaration.AppendLine("_never = 0,");
            // TODO setup primitive types?

            code.TypeDeclarations.AppendLine("// Type Declarations");
            code.FunctionDeclarations.AppendLine("// Function Declarations");
            code.StructDeclarations.AppendLine("// Struct Declarations");
            code.GlobalDefinitions.AppendLine("// Global Definitions");
            code.Definitions.AppendLine("// Definitions");
        }

        public void EmitEntryPointAdapter(FunctionIL entryPoint, Code code)
        {
            if (entryPoint is null) return;

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine("// Entry Point Adapter");
            code.Definitions.AppendLine("int32_t main(const int argc, char const * const * const argv)");
            code.Definitions.BeginBlock();
            var arguments = new List<string>();
            foreach (var parameterType in entryPoint.Parameters.Select(p => p.DataType).Cast<ObjectType>())
            {
                if (parameterType.ContainingNamespace == SystemConsole
                    && parameterType.Name == "Console")
                {
                    code.Definitions.AppendLine(
                        "system__console__Console console = { &system__console__Console___vtable, malloc(sizeof(system__console__Console___Self)) };");
                    arguments.Add("system__console__Console___new__1(console)");
                }
                else if (parameterType.ContainingNamespace == SystemConsole
                         && parameterType.Name == "Arguments")
                    throw new NotImplementedException();
                else
                    throw new Exception($"Unexpected type for parameter to main: {parameterType}");
            }
            var joinedArguments = string.Join(", ", arguments);
            if (entryPoint.Symbol.ReturnDataType == DataType.Void)
            {
                code.Definitions.AppendLine($"{nameMangler.Mangle(entryPoint.Symbol)}({joinedArguments});");
                code.Definitions.AppendLine("return 0;");
            }
            else
                code.Definitions.AppendLine($"return {nameMangler.Mangle(entryPoint.Symbol)}({joinedArguments})._value;");

            code.Definitions.EndBlock();
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public void EmitPostamble(Code code)
        {
            // Close the Type_ID enum
            code.TypeIdDeclaration.EndBlockWithSemicolon();
            code.TypeIdDeclaration.AppendLine("typedef enum _Type_ID _Type_ID;");
        }

        private static readonly NamespaceName SystemConsole = new NamespaceName("system", "console");
    }
}
