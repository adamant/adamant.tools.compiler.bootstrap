using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ControlFlowEmitter : IEmitter<ControlFlowGraph>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<DataType> typeConverter;

        public ControlFlowEmitter(
            NameMangler nameMangler,
            IConverter<DataType> typeConverter)
        {
            this.typeConverter = typeConverter;
            this.nameMangler = nameMangler;
        }

        public void Emit(ControlFlowGraph cfg, Code code)
        {
            var definitions = code.Definitions;

            foreach (var variable in cfg.VariableDeclarations.Where(v => v.Exists))
                EmitVariable(variable, definitions);

            if (cfg.VariableDeclarations.Any(v => v.Exists))
                definitions.BlankLine();

            var voidReturn = cfg.ReturnType == DataType.Void;
            foreach (var block in cfg.BasicBlocks)
                EmitBlock(block, voidReturn, definitions);
        }

        private void EmitVariable(LocalVariableDeclaration variable, CCodeBuilder code)
        {
            Requires.That(nameof(variable), variable.Exists, "tried to look up variable that does not exist");
            var initializer = variable.IsParameter ? $" = {nameMangler.Mangle(variable.Name)}" : "";
            code.AppendLine($"{typeConverter.Convert(variable.Type)} _{NameOf(variable.Reference)}{initializer}; // {variable}");
        }

        private static string NameOf(VariableReference variable)
        {
            return variable.VariableNumber == 0 ? "result" : variable.VariableNumber.ToString();
        }

        private void EmitBlock(BasicBlock block, bool voidReturn, CCodeBuilder code)
        {
            code.AppendLine($"bb{block.Number}:");
            code.BeginBlock();
            foreach (var statement in block.Statements) EmitStatement(statement, voidReturn, code);
            code.EndBlock();
        }

        private void EmitStatement(Statement statement, bool voidReturn, CCodeBuilder code)
        {
            code.AppendLine("// " + statement);
            switch (statement)
            {
                case IfStatement ifStatement:
                    code.AppendLine($"if({ConvertValue(ifStatement.Condition)}._value) goto bb{ifStatement.ThenBlockNumber}; else goto bb{ifStatement.ElseBlockNumber};");
                    break;
                case GotoStatement gotoStatement:
                    code.AppendLine($"goto bb{gotoStatement.GotoBlockNumber};");
                    break;
                case ReturnStatement _:
                    code.AppendLine(voidReturn ? "return;" : "return _result;");
                    break;
                case AssignmentStatement assignment:
                    code.AppendLine(
                        $"{ConvertPlace(assignment.Place)} = {ConvertValue(assignment.Value)};");
                    break;
                case ActionStatement action:
                    code.AppendLine(ConvertValue(action.Value) + ";");
                    break;
                case DeleteStatement deleteStatement:
                {
                    var self = ConvertValue(deleteStatement.Place);
                    var typeName = nameMangler.Mangle(deleteStatement.Type);
                    // TODO once deletes are implemented, call them
                    //code.AppendLine($"{self}._vtable->{typeName}___delete__1({self});");
                    code.AppendLine($"free({self}._self);");
                    break;
                }
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private string ConvertPlace(Place place)
        {
            switch (place)
            {
                case VariableReference variable:
                    return "_" + NameOf(variable);
                default:
                    throw NonExhaustiveMatchException.For(place);
            }
        }

        private string ConvertValue(Value value)
        {
            switch (value)
            {
                case IntegerConstant integer:
                    return $"({ConvertType(integer.Type)}){{{integer.Value}}}";
                case Utf8BytesConstant utf8BytesConstant:
                    return $"((_byte*)u8\"{utf8BytesConstant.Value.Escape()}\")";
                case BooleanConstant boolean:
                    var booleanValue = boolean.Value ? 1 : 0;
                    return $"({ConvertType(boolean.Type)}){{{booleanValue}}}";
                case FunctionCall functionCall:
                {
                    var mangledName = nameMangler.Mangle(functionCall.FunctionName);
                    var arguments = functionCall.Self.YieldValue().Concat(functionCall.Arguments).Select(ConvertValue);
                    return $"{mangledName}__{functionCall.Arity}({string.Join(", ", arguments)})";
                }
                case ConstructorCall constructorCall:
                {
                    var typeName = nameMangler.Mangle(constructorCall.Type);
                    var selfArgument = $"({typeName}){{&{typeName}___vtable, malloc(sizeof({typeName}___Self))}}";
                    var arguments = selfArgument.YieldValue().Concat(constructorCall.Arguments.Select(ConvertValue));
                    return $"{typeName}___new__{constructorCall.Arity}({string.Join(", ", arguments)})";
                }
                case DeclaredValue declaredValue:
                    return nameMangler.Mangle(declaredValue.Name);
                case FieldAccessValue fieldAccess:
                    return $"{ConvertValue(fieldAccess.Expression)}.{nameMangler.Mangle(fieldAccess.Field.UnqualifiedName)}";
                case Place place:
                    return ConvertPlace(place);
                case VirtualFunctionCall virtualCall:
                {
                    var self = ConvertValue(virtualCall.Self);
                    var mangledName = nameMangler.Mangle(virtualCall.FunctionName);
                    var arity = virtualCall.Arguments.Count + 1;
                    var arguments = virtualCall.Arguments.Select(ConvertValue).Prepend(self);
                    return $"{self}._vtable->{mangledName}__{arity}({string.Join(", ", arguments)})";
                }
                case BinaryOperation binaryOperation:
                {
                    var left = ConvertValue(binaryOperation.LeftOperand);
                    var right = ConvertValue(binaryOperation.RightOperand);
                    var operationType = nameMangler.Mangle(binaryOperation.Type);
                    string @operator;
                    switch (binaryOperation.Operator)
                    {
                        // If a binary operator was emitting for a boolean operation,
                        // then it doesn't short circuit, we just call the function
                        case BinaryOperator.And:
                            @operator = "and";
                            break;
                        case BinaryOperator.Or:
                            @operator = "or";
                            break;
                        case BinaryOperator.Plus:
                            @operator = "add";
                            break;
                        case BinaryOperator.Minus:
                            @operator = "sub";
                            break;
                        case BinaryOperator.Asterisk:
                            @operator = "mul";
                            break;
                        case BinaryOperator.Slash:
                            @operator = "div";
                            break;
                        case BinaryOperator.EqualsEquals:
                            @operator = "eq";
                            break;
                        case BinaryOperator.NotEqual:
                            @operator = "ne";
                            break;
                        case BinaryOperator.LessThan:
                            @operator = "lt";
                            break;
                        case BinaryOperator.LessThanOrEqual:
                            @operator = "lte";
                            break;
                        case BinaryOperator.GreaterThan:
                            @operator = "gt";
                            break;
                        case BinaryOperator.GreaterThanOrEqual:
                            @operator = "gte";
                            break;
                        default:
                            throw NonExhaustiveMatchException.ForEnum(binaryOperation.Operator);
                    }
                    return $"{operationType}__{@operator}({left}, {right})";
                }
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private string ConvertType(DataType type)
        {
            switch (type)
            {
                case ObjectType objectType:
                    return nameMangler.Mangle(objectType);
                case SimpleType simpleType:
                    return nameMangler.Mangle(simpleType.Name);
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
