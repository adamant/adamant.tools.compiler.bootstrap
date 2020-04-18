# Adamant.Tools.Compiler.Bootstrap.AST

The current architecture uses a single AST that results from parsing for name binding and type checking. It follows the strategy of transforming toward higher level representations as fast as possible. The parser generates things needed for later analysis like names and also simplifies slightly. For example, the parser drops parentheses out of the tree. Borrow checking is performed on an intermediate language (IR) representation of functions. However, this is attached directly to the AST nodes for functions.

Both tokens and AST nodes have a separate interface and implementation layer. Other parts of the compiler deal only with the interfaces. For both the tokens and AST, transitioning to this separation brought a lot of improvement. Even though it produces more code, the flexibility of multiple inheritance hierarchies is very powerful and enables stronger typing.

The AST is designed to be as concrete as possible. Meaning that each different kind of syntax construct has a separate node type. For example, it used to be the case that functions, methods, and associated functions were all the same node type. Now, each has a different node type. This has improved type safety by ensuring nodes have exactly the properties that make sense for them and that those properties have correct nullability.

## Naming Conventions

Each AST class ends in `Syntax` to distinguish them from intermediate language nodes. The category of the node is also in the name. While including the category is more verbose, not doing so leads to inconsistencies. There are always some node types for which leaving out the category produces a confusing name. Categories are:

* Declaration
* * Statement
* Expression
  * OperatorExpression
  * LiteralExpression
  * ConversionExpression
  * InvocationExpression
* Parameter
* Type

## Traversing the AST

AST traversal is done either with a tree walker or by matching on node type. Tree walkers are similar to the listener types of ANTLR. They provide an internal traversal. That is, the nodes are walked and the methods of the walker are called as nodes are visited. Sections of the tree can be skipped by either not implementing the interface for those kind of nodes or by indicating nodes to skip over via the `ShouldSkip()` method. The other, more common way of traversing the AST is simply by matching on node type and recursing down the tree. When doing this, having more generic/broad switches can help to insulate traversals from AST changes. That is, if a traversal relies on a single large switch over all node types rather than on separate methods with separate switches for different node categories, fewer changes are required when the AST types change.

It took a while to arrive at these as the traversal approach. Methods on the nodes with overrides were ruled out early since there are many traversals and they are too hard to work on if they are spread out among all the node classes. Instead, the visitor pattern was originally the preferred method of traversing the AST. This was very cumbersome though. It also made it very difficult to handle multiple node types as a single case. At one point, there was experimentation with using reflection to safely implement visitors but that was confusing to refactoring tools and spread the code out among many methods just like a visitor. Using switch statements to match on type was a near ideal syntax, but without exhaustiveness checking, it was incredibly error prone. With the creation and use of the `ExhaustiveMatching.Analyzer` package that provides the most flexible exhaustiveness checking of any language, that has changed. Now switching on type is the preferred approach in many cases.

## Design Goals

### Adaptability to Syntax Changes

As the language is still being designed, it is important that the lexer, parser and concrete syntax tree be easy to modify. This is supported by keeping them as simple as possible. Code generation is used around tokens so that tokens can be easily added and removed. Several points in the lexer use generated lists of all tokens of a given type to guide the lexing process.

### Correct by Construction

Originally, tokens were not as strongly typed. They were using several struct types with an enum for the token type. This performance optimization was modelled on the Roslyn C# compiler. However, it was decided that having strongly typed tokens was more valuable and the transition was made to the current set of classes. This also allowed for more sophisticated type relationships such as a base class for keywords and a base class for operators. Ideally, missing tokens would be represented with a special token that included the type and position of the missing token. However, the C# type system didn't offer good options for representing that in a strongly typed way. It was decided to use `null` to represent missing tokens and accept the limitations that might imply.

## Design History

The design of the compiler abstract syntax tree (AST) has gone through many revisions. Initially, it was hoped there could be support for incremental compilation through reuse of unchanged portions of the syntax tree. That is the strategy taken by the Roslyn C# compiler. Eventually, it was decided that this was too complex. It is more important to get a compiler up and working. Even after this change of direction, the syntax tree was still very concrete, containing every token from the source file except whitespace, comments and unexpected tokens. In time it was realized that was a hold over and not needed. It was creating a lot of boilerplate code that wasn't being used by later phases. The Rust compiler throws way as much information as possible in the parsing phase and builds a fairly abstract syntax tree with file positions for only the most important elements.

At that time, there was still a separate "analysis" tree that largely mirrored the syntax tree. This was used for name binding, type checking and any other analysis. The purpose of this was to make the analysis phase more strongly typed. However, all of these fields still had to be nullable so that the analysis could fill them in. Looking at the Rust compiler, it performs name binding on the syntax tree (except for member accesses etc. that require type information to resolved). They then transform it to their "high-level IR" for type checking. This tree simplifies a number of things like loops etc. Then they transform to "mid-level IR" for borrow checking. That means lots of types that mirror existing types. In C# just declaring types like that has a lot of boilerplate. It was decided that rather than separate these out for the sake of "purity" or incremental compilation that probably won't be done in this compiler, they should be collapsed for speed of development.
