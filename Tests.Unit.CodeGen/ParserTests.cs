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

            Assert.Equal(Symbol("MyBase"), config.BaseType);
        }

        [Fact]
        public void ParsesQuotedBaseType()
        {
            const string grammar = "◊base 'MyBase';";

            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Equal(QuotedSymbol("MyBase"), config.BaseType);
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

            Assert.Equal(FixedList("Foo.Bar", "Foo.Bar.Baz"), config.UsingNamespaces);
        }
        #endregion

        #region Rules
        [Fact]
        public void ParsesSimpleNonterminalRule()
        {
            const string grammar = "SubType;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(Symbol("SubType"), rule.Nonterminal);
            Assert.Empty(rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesSimpleQuotedNonterminalRule()
        {
            const string grammar = "'IMyFullTypeName';";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(QuotedSymbol("IMyFullTypeName"), rule.Nonterminal);
            Assert.Empty(rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesSimpleNonterminalRuleWithDefaultBaseType()
        {
            const string grammar = "◊base MyBase;\nSubType;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(Symbol("SubType"), rule.Nonterminal);
            var expectedParents = FixedList(Symbol("MyBase"));
            Assert.Equal(expectedParents, rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesBaseTypeRule()
        {
            const string grammar = "◊base MyBase;\nMyBase;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(Symbol("MyBase"), rule.Nonterminal);
            Assert.Empty(rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesSingleInheritanceRule()
        {
            const string grammar = "SubType: BaseType;";

            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(Symbol("SubType"), rule.Nonterminal);
            Assert.Single(rule.Parents, Symbol("BaseType"));
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParsesMultipleInheritanceRule()
        {
            const string grammar = "SubType: BaseType1, BaseType2;";

            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(Symbol("SubType"), rule.Nonterminal);
            var expectedParents = FixedList(Symbol("BaseType1"), Symbol("BaseType2"));
            Assert.Equal(expectedParents, rule.Parents);
            Assert.Empty(rule.Properties);
        }

        [Fact]
        public void ParseQuotedInheritanceRule()
        {
            const string grammar = "SubType: 'BaseType1', BaseType2;";

            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            Assert.Equal(new GrammarSymbol("SubType"), rule.Nonterminal);
            var expectedParents = FixedList(QuotedSymbol("BaseType1"), Symbol("BaseType2"));
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

        #region Comments
        [Fact]
        public void Ignores_line_comments()
        {
            const string grammar = "// A comment";
            var config = Parser.ReadGrammarConfig(grammar);

            Assert.Empty(config.Rules);
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
            Assert.Equal(Type(Symbol("MyProperty")), property.Type);
        }

        [Fact]
        public void ParsesSimpleOptionalProperty()
        {
            const string grammar = "MyNonterminal = MyProperty?;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal(OptionalType(Symbol("MyProperty")), property.Type);
        }

        [Fact]
        public void ParsesTypedProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal(Type(Symbol("MyType")), property.Type);
        }

        [Fact]
        public void ParsesQuotedTypedProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:'MyType';";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal(Type(QuotedSymbol("MyType")), property.Type);
        }

        [Fact]
        public void ParsesListTypedProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType*;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal(ListType(Symbol("MyType")), property.Type);
        }

        [Fact]
        public void ParsesOptionalTypeProperty()
        {
            const string grammar = "MyNonterminal = MyProperty:MyType?;";
            var config = Parser.ReadGrammarConfig(grammar);

            var rule = Assert.Single(config.Rules);
            var property = Assert.Single(rule.Properties);
            Assert.Equal("MyProperty", property.Name);
            Assert.Equal(OptionalType(Symbol("MyType")), property.Type);
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
                Assert.Equal(Type(Symbol("MyType1")), p1.Type);
            }, p2 =>
            {
                Assert.Equal("MyProperty2", p2.Name);
                Assert.Equal(ListType(Symbol("MyType2")), p2.Type);
            });
        }

        [Fact]
        public void ParseErrorDuplicateProperties()
        {
            const string grammar = "MyNonterminal = Something Something:'Blah';";

            var ex = Assert.Throws<FormatException>(() => Parser.ReadGrammarConfig(grammar));

            Assert.Equal("Rule for MyNonterminal contains duplicate property definitions", ex.Message);
        }
        #endregion

        private static GrammarSymbol Symbol(string text)
        {
            return new GrammarSymbol(text);
        }

        private static GrammarSymbol QuotedSymbol(string text)
        {
            return new GrammarSymbol(text, true);
        }

        private static GrammarType Type(GrammarSymbol symbol)
        {
            return new GrammarType(symbol, false, false);
        }

        private static GrammarType OptionalType(GrammarSymbol symbol)
        {
            return new GrammarType(symbol, true, false);
        }

        private static GrammarType ListType(GrammarSymbol symbol)
        {
            return new GrammarType(symbol, false, true);
        }

        private static FixedList<T> FixedList<T>(params T[] values)
        {
            return new FixedList<T>(values);
        }
    }
}
