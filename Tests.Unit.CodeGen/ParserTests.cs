using System;
using Adamant.Tools.Compiler.Bootstrap.CodeGen;
using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.CodeGen
{
    [Trait("Category", "CodeGen")]
    public class ParserTests
    {
        #region Options
        [Fact]
        public void DefaultsNamespaceToNull()
        {
            const string grammar = "";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Null(config.Namespace);
        }

        [Fact]
        public void DefaultsBaseTypeToNull()
        {
            const string grammar = "";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Null(config.BaseType);
        }

        [Fact]
        public void DefaultsPrefixToEmptyString()
        {
            const string grammar = "";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("", config.Prefix);
        }

        [Fact]
        public void DefaultsSuffixToEmptyString()
        {
            const string grammar = "";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("", config.Suffix);
        }

        [Fact]
        public void DefaultsListTypeToList()
        {
            const string grammar = "";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("List", config.ListType);
        }

        [Fact]
        public void ParsesNamespace()
        {
            const string grammar = "◊namespace Foo.Bar.Baz;";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("Foo.Bar.Baz", config.Namespace);
        }

        [Fact]
        public void ParsesBaseType()
        {
            const string grammar = "◊base MyBase;";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal(new Symbol("MyBase"), config.BaseType);
        }

        [Fact]
        public void ParsesQuotedBaseType()
        {
            const string grammar = "◊base 'MyBase';";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal(new Symbol("MyBase", true), config.BaseType);
        }

        [Fact]
        public void ParsesPrefix()
        {
            const string grammar = "◊prefix MyPrefix;";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("MyPrefix", config.Prefix);
        }

        [Fact]
        public void ParsesSuffix()
        {
            const string grammar = "◊suffix MySuffix;";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("MySuffix", config.Suffix);
        }

        [Fact]
        public void ParsesListType()
        {
            const string grammar = "◊list MyList;";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal("MyList", config.ListType);
        }

        [Fact]
        public void ParsesUsingNamespaces()
        {
            const string grammar = "◊using Foo.Bar;\r◊using Foo.Bar.Baz;";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal(new[] { "Foo.Bar", "Foo.Bar.Baz" }, config.UsingNamespaces);
        }
        #endregion

        #region Rules
        [Fact]
        public void ParsesSimpleNonterminalRule()
        {
            const string grammar = "SubType;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("SubType"), rule.Nonterminal);
            Assert.Empty(rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesSimpleQuotedNonterminalRule()
        {
            const string grammar = "'IMyFullTypeName';";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("IMyFullTypeName", true), rule.Nonterminal);
            Assert.Empty(rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesSimpleNonterminalRuleWithDefaultBaseType()
        {
            const string grammar = "◊base MyBase;\nSubType;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("SubType"), rule.Nonterminal);
            var expectedParents = new[] { new Symbol("MyBase") }.ToFixedList();
            Assert.Equal(expectedParents, rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesBaseTypeRule()
        {
            const string grammar = "◊base MyBase;\nMyBase;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("MyBase"), rule.Nonterminal);
            Assert.Empty(rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesSingleInheritanceRule()
        {
            const string grammar = "SubType: BaseType;";

            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("SubType"), rule.Nonterminal);
            Assert.Single(rule.Parents, new Symbol("BaseType"));
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesMultipleInheritanceRule()
        {
            const string grammar = "SubType: BaseType1, BaseType2;";

            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("SubType"), rule.Nonterminal);
            var expectedParents = new[] { new Symbol("BaseType1"), new Symbol("BaseType2") }.ToFixedList();
            Assert.Equal(expectedParents, rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParseQuotedInheritanceRule()
        {
            const string grammar = "SubType: 'BaseType1', BaseType2;";

            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new Symbol("SubType"), rule.Nonterminal);
            var expectedParents = new[] { new Symbol("BaseType1", true), new Symbol("BaseType2") }.ToFixedList();
            Assert.Equal(expectedParents, rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParseErrorTooManyEqualSigns()
        {
            const string grammar = "NonTerminal = Foo = Bar;";

            var ex = Assert.Throws<FormatException>(() => Parser.ReadGrammarConfig(grammar));

            Assert.Equal("Too many equal signs on line: 'NonTerminal = Foo = Bar'", ex.Message);
        }

        [Fact]
        public void ParseErrorTooManyColonsInDeclaration()
        {
            const string grammar = "SubType: BaseType: What = Foo;";

            var ex = Assert.Throws<FormatException>(() => Parser.ReadGrammarConfig(grammar));

            Assert.Equal("Too many colons in declaration: 'SubType: BaseType: What '", ex.Message);
        }
        #endregion

        #region Properties
        [Fact]
        public void ParsesSimpleProperty()
        {
            const string grammar = "MyNonterminal = MyProperty;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal("MyProperty", property.Type);
            Assert.False(property.IsOptional);
            Assert.False(property.IsList);
        }

        [Fact]
        public void ParsesSimpleOptionalProperty()
        {
            const string grammar = "MyNonterminal = MyProperty?;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal("MyProperty", property.Type);
            Assert.True(property.IsOptional);
            Assert.False(property.IsList);
        }

        [Fact]
        public void ParsesTypedProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal("MyType", property.Type);
            Assert.False(property.IsOptional);
            Assert.False(property.IsList);
        }

        [Fact]
        public void ParsesListTypedProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType*;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal("MyType", property.Type);
            Assert.False(property.IsOptional);
            Assert.True(property.IsList);
        }

        [Fact]
        public void ParsesOptionalTypeProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType?;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal("MyType", property.Type);
            Assert.True(property.IsOptional);
            Assert.False(property.IsList);
        }

        [Fact]
        public void ParseErrorTooManyColonsInDefinition()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType:What;";

            var ex = Assert.Throws<FormatException>(() => Parser.ReadGrammarConfig(grammar));

            Assert.Equal("Too many colons in definition: 'MyProperty:MyType:What'", ex.Message);
        }

        [Fact]
        public void ParsesMultipleProperties()
        {
            const string grammar = "MyNonterminal = MyProperty1:MyType1 MyProperty2:MyType2*;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Collection(rule.Properties, p1 =>
            {
                Assert.Equal("MyProperty1", p1.Name);
                Assert.Equal("MyType1", p1.Type);
                Assert.False(p1.IsList);
            }, p2 =>
            {
                Assert.Equal("MyProperty2", p2.Name);
                Assert.Equal("MyType2", p2.Type);
                Assert.True(p2.IsList);
            });
        }
        #endregion
    }
}
