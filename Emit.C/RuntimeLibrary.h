#include <stdint.h>

#define UNUSED(x) (void)(x)

typedef struct { int32_t ₐvalue; } ₐint;
typedef struct { uint32_t ₐvalue; } ₐuint;
typedef struct { uint8_t ₐvalue; } ₐbyte;
typedef struct { uintptr_t ₐvalue; } ₐsize;
typedef struct { intptr_t ₐvalue; } ₐoffset;

/*
For now, we have moved strings into the runtime library
*/
typedef struct {
    ₐsize byte_count;
    ₐbyte const* bytes;
} ᵢString;

inline ᵢString ᵢString·ₐoperator_string_literal(ₐsize byte_count, ₐbyte const* bytes)
{
    return (ᵢString){ byte_count, bytes };
}
