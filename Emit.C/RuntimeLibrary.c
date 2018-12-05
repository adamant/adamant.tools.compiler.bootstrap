#include "RuntimeLibrary.h"
#include <stdio.h>
//#include <windows.h>

typedef void* LPVOID;
typedef LPVOID HANDLE;
typedef size_t SIZE_T;
typedef uint32_t DWORD;
typedef int32_t BOOL;

HANDLE GetProcessHeap();
LPVOID HeapAlloc(HANDLE hHeap, DWORD dwFlags, SIZE_T dwBytes);
LPVOID HeapReAlloc(HANDLE hHeap, DWORD dwFlags, LPVOID lpMem, SIZE_T dwBytes);
BOOL HeapFree(HANDLE hHeap, DWORD dwFlags, LPVOID lpMem);
DWORD GetLastError();

void test()
{
    void* ptr = HeapAlloc(GetProcessHeap(), 0, 10);
    HeapFree(GetProcessHeap(), 0, ptr);
    //void* ptr = aligned_alloc(8, 32);
    //free(ptr);
}

// Inline functions from RuntimeLibrary.h
extern String String___op_string_literal__2(_size count, _byte* bytes);


void print_string__1(String text)
{
    printf("%.*s\n", (int)text.byte_count._value, (char*)text.bytes);
}
