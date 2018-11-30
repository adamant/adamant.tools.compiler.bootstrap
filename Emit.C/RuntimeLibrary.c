#include "RuntimeLibrary.h"
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
extern ᵢString ᵢString·ₐoperator_string_literal(ₐsize count, ₐbyte const* bytes);