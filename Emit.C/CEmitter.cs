using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Emit.C.Tests;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CEmitter : Emitter
    {
        private readonly NameMangler nameMangler = new NameMangler();

        public override string Emit(Package package)
        {
            var code = new Code();

            EmitPreamble(code);

            foreach (var compilationUnit in package.CompilationUnits)
                Emit(compilationUnit, code);

            EmitEntryPointAdapter(code, package);

            EmitPostamble(code);

            return code.ToString();
        }

        #region Emit Semantic Nodes
        internal void EmitPreamble(Code code)
        {
            // Setup the beginning of each section
            code.Includes.AppendLine("#include \"RuntimeLibrary.h\"");

            code.TypeIdDeclaration.AppendLine("// Type ID Declarations");
            code.TypeIdDeclaration.AppendLine("enum Type_ID");
            code.TypeIdDeclaration.BeginBlock();
            // TODO setup primitive types?

            code.TypeDeclarations.AppendLine("// Type Declarations");
            code.FunctionDeclarations.AppendLine("// Function Declarations");
            code.ClassDeclarations.AppendLine("// Class Declarations");
            code.GlobalDefinitions.AppendLine("// Global Definitions");
            code.Definitions.AppendLine("// Definitions");
        }

        internal void Emit(CompilationUnit compilationUnit, Code code)
        {
            foreach (var declaration in compilationUnit.Declarations)
                Emit(declaration, code);
        }

        internal void Emit(Declaration declaration, Code code)
        {
            Match.On(declaration).With(m => m
                .Is<FunctionDeclaration>(f =>
                {
                    var name = nameMangler.FunctionName(f);
                    var parameters = Convert(f.Parameters);
                    var returnType = Convert(f.ReturnType.Type);

                    // Write out the function declaration for C so we can call functions defined after others
                    code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

                    code.Definitions.DeclarationSeparatorLine();
                    code.Definitions.AppendLine($"{returnType} {name}({parameters})");
                    Emit(code, f.Body);
                }));
        }

        internal void Emit(Code code, Block block)
        {
            code.Definitions.BeginBlock();
            code.Definitions.EndBlock();
        }
        #endregion

        #region Convert
        private string Convert(DataType type)
        {
            return MatchInto<string>.On(type).With(m => m
                .Is<PrimitiveType>(p =>
                {
                    switch (p.Kind)
                    {
                        case PrimitiveTypeKind.Void:
                            return "void";
                        case PrimitiveTypeKind.Int:
                            return "int32";
                        default:
                            throw new NotImplementedException(p.Kind.ToString());
                    }
                }));
        }

        private string Convert(IEnumerable<Parameter> parameters)
        {
            return string.Join(", ", parameters.Select(Convert));
        }

        private string Convert(Parameter parameter)
        {
            if (parameter.MutableBinding)
                return $"{Convert(parameter.Type.Type)} {parameter.Name}";
            else
                return $"const {Convert(parameter.Type.Type)} {parameter.Name}";
        }
        #endregion

        internal void EmitEntryPointAdapter(Code code, Package package)
        {
            // TODO implement
        }

        internal void EmitPostamble(Code code)
        {
            // Close the Type_ID enum
            code.TypeIdDeclaration.EndBlockWithSemicolon();
            code.TypeIdDeclaration.AppendLine("typedef enum Type_ID Type_ID;");
        }
    }
}
