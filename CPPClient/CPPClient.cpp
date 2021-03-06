// CPPClient.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include "pch.h"
#include <iostream>
#include "Windows.h"

typedef wchar_t* (__cdecl *AppProc)();
typedef void (__cdecl *AppProcWinInitialize)();
typedef void(__cdecl *AppProcRegisterStoreService)();
void CallUWPFeatures()
{
	HINSTANCE hinstLib; 
	BOOL fFreeResult;
	AppProcWinInitialize appProcWinInitialize;
	AppProc appProc;
	AppProcRegisterStoreService appProcStoreService;
	int i;
	hinstLib = LoadLibrary(TEXT("UWPFeatures.dll"));
	if (NULL != hinstLib)
	{
		appProcWinInitialize = (AppProcWinInitialize)GetProcAddress(hinstLib, "WinInitialize");
		if (NULL != appProcWinInitialize)
		{
			appProcWinInitialize();
			/*
			appProc = (AppProc)GetProcAddress(hinstLib, "RegisterBackgroundTask");
			if (NULL != appProc)
			{
				std::wcout << (appProc)() << std::endl;
			}
			*/
			appProcStoreService = (AppProcRegisterStoreService)GetProcAddress(hinstLib, "RegisterStoreNotification");
			if (NULL != appProcStoreService)
			{
				(appProcStoreService)();
				std::wcout << L"Register Completed" << std::endl;
			}
			else

			{
				std::wcout << L"RegisterStoreNotification not Completed" << std::endl;
			}
		}
		std::cin >> i;
		fFreeResult = FreeLibrary(hinstLib);
	}
	else
	{
		std::cout << "Could not find UWPFeatures.dll\n";
	}
}
void handle_eptr(std::exception_ptr eptr) // passing by value is ok
{
	try {
		if (eptr) {
			std::rethrow_exception(eptr);
		}
	}
	catch (const std::exception& e) {
		std::cout << "Caught exception \"" << e.what() << "\"\n";
	}
}


int main()
{
	int i;
	std::exception_ptr eptr;
    std::cout << "Start\n"; 
	try {
		 
		CallUWPFeatures();
	}
	catch (const std::exception& e)
	{
		std::cerr << e.what() << std::endl;
	}
	catch (...)
	{	
		std::cout << "unknown error\n";
		eptr = std::current_exception();		
	}
	handle_eptr(eptr);
	std::cout << "End\n";
	std::cin >> i;
} 