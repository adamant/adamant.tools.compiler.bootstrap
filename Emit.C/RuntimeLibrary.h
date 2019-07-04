#include <stdint.h>

// Rather than including all of stdlib.h we only declare the one thing we need.
void* malloc(size_t size);

#define _UNUSED(x) (void)(x)

// `bool` type
typedef struct { _Bool _value; } _bool;

inline _bool _bool__and(_bool x, _bool y) { return (_bool) { x._value& y._value }; }
inline _bool _bool__or(_bool x, _bool y) { return (_bool) { x._value | y._value }; }

// Integer Type Operations Macro
#define INTEGER_OPERATIONS(type) \
inline type type##__add(type x, type y) { return (type) { x._value + y._value }; } \
inline type type##__sub(type x, type y) { return (type) { x._value - y._value }; } \
inline type type##__mul(type x, type y) { return (type) { x._value* y._value }; } \
inline type type##__div(type x, type y) { return (type) { x._value / y._value }; } \
inline type type##__remainder__1(type x, type y) { return (type) { x._value% y._value }; } \
inline _bool type##__eq(type x, type y) { return (_bool) { x._value == y._value }; } \
inline _bool type##__ne(type x, type y) { return (_bool) { x._value != y._value }; } \
inline _bool type##__lt(type x, type y) { return (_bool) { x._value < y._value }; } \
inline _bool type##__lte(type x, type y) { return (_bool) { x._value <= y._value }; } \
inline _bool type##__gt(type x, type y) { return (_bool) { x._value > y._value }; } \
inline _bool type##__gte(type x, type y) { return (_bool) { x._value >= y._value }; }

// `int` type
typedef struct { int32_t _value; } _int;

INTEGER_OPERATIONS(_int)

// `uint` type
typedef struct { uint32_t _value; } _uint;

INTEGER_OPERATIONS(_uint)

// `byte` type
typedef struct { uint8_t _value; } _byte;

INTEGER_OPERATIONS(_byte)

// `size` type
typedef struct { uintptr_t _value; } _size;

INTEGER_OPERATIONS(_size)

// `offset` type
typedef struct { intptr_t _value; } _offset;

INTEGER_OPERATIONS(_offset)

// `String` type
// Note: For now, we have moved strings
typedef struct
{
    _size byte_count;
    _byte* bytes;
} String;

inline String String___op_string_literal__2(_size byte_count, _byte* bytes)
{
    return (String) { byte_count, bytes };
}

// Direct support through the runtime for now
void print_string__1(String text);
String _uint__to_display_string__0(_uint value);
