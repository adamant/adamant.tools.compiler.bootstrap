#include <stdint.h>

// Rather than including all of stdlib.h we only declare the one thing we need.
void* malloc(size_t size);

#define _UNUSED(x) (void)(x)

typedef struct { int32_t _value; } _int;
typedef struct { uint32_t _value; } _uint;
typedef struct { uint8_t _value; } _byte;
typedef struct { uintptr_t _value; } _size;
typedef struct { intptr_t _value; } _offset;

/*
For now, we have moved strings and console IO into the runtime library
*/
typedef struct {
    _size byte_count;
    _byte* bytes;
} String;

inline String String___op_string_literal__2(_size byte_count, _byte* bytes)
{
    return (String){ byte_count, bytes };
}

void print_string__1(String text);
