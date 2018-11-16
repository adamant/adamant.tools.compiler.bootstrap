# Adamant.Tools.Compiler.Bootstrap.Tokens

## Naming Conventions

There are a number of reasons that token naming is challenging:

* Operators can be overloaded so that `*` may not mean multiply for example.
* Operators and keywords are used in multiple contexts where they have different meanings. For example, `-` could be subtract or negate.
* Characters have multiple names. For example, `#` is called "number sign", "hash sign", or "pound sign", or even "octothorpe."

So while the temptation to name tokens after what they mean is strong, the policy adapted by the Roslyn project makes more sense. Tokens are named after their characters. All keyword tokens are simply the keyword followed by "KeywordToken". All tokens composed of symbols are just the concatenation of the names of the individual characters followed by "Token". For characters, the Unicode official name is generally used. Filler words like "sign" are dropped unless they are needed to disambiguate from another token. However, occasionally, the unicode character names are very strange and a more common name is used.

Some tokens in Adamant have multiple acceptable character sequences. For example, both `->` (U+002D, U+003E) and `→` (U+2192) are acceptable. In these cases, the token is named after the shorter character the longer sequence is meant to be evocative of.

## Software Architecture

Ideally tokens would be enums (like Adamant enum structs). That would maximize performance and minimize memory footprint. Since C# enums are not flexible enough, this can't be done. Instead switching on type is used.

Because the different tokens can be grouped many different ways into complex hierarchies, it became necessary to use interfaces to represent this. In time, the mix of classes and interfaces became confusing and limiting. For that reason, the tokens have been fully encapsulated in this project with only their interfaces exposed. This allows the equivalent of the Adamant ability to implement the interface of a class.
