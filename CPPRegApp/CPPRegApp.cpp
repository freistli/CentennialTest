// CPPRegApp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>

using namespace std;

//Refer to https://advancedcppwithexamples.blogspot.com/2009/08/reading-and-writing-from-windows.html
void writeToRegistry(void)
{
	DWORD lRv;
	HKEY hKey;

	//Check if the registry exists
	//http://msdn.microsoft.com/en-us/library/ms724897(VS.85).aspx
	lRv = RegOpenKeyEx(
		HKEY_CURRENT_USER,
		L"Software\\Zahid",
		0,
		KEY_WRITE,
		&hKey
	);

	if (lRv != ERROR_SUCCESS)
	{
		cout << "\nDoesn't find key. Write it.";
		DWORD dwDisposition;

		// Create a key if it did not exist
		//http://msdn.microsoft.com/en-us/library/ms724844(VS.85).aspx
		lRv = RegCreateKeyEx(
			HKEY_CURRENT_USER,
			L"Software\\Zahid", //"Use Multi-Byte Character Set" by using L
			0,
			NULL,
			REG_OPTION_NON_VOLATILE,
			KEY_ALL_ACCESS,
			NULL,
			&hKey,
			&dwDisposition
		);

		DWORD dwValue = 1;

		//http://msdn.microsoft.com/en-us/library/ms724923(VS.85).aspx
		RegSetValueEx(
			hKey,
			L"Something",
			0,
			REG_DWORD,
			reinterpret_cast<BYTE *>(&dwValue),
			sizeof(dwValue)
		);

		//http://msdn.microsoft.com/en-us/library/ms724837(VS.85).aspx
		RegCloseKey(hKey);
	}
}

void readValueFromRegistry(void)
{
	//Example from http://msdn.microsoft.com/en-us/library/ms724911(VS.85).aspx

	HKEY hKey;

	//Check if the registry exists
	DWORD lRv = RegOpenKeyEx(
		HKEY_CURRENT_USER,
		L"Software\\Zahid",
		0,
		KEY_READ,
		&hKey
	);

	if (lRv == ERROR_SUCCESS)
	{
		DWORD BufferSize = sizeof(DWORD);
		DWORD dwRet;
		DWORD cbData;
		DWORD cbVal = 0;

		dwRet = RegQueryValueEx(
			hKey,
			L"Something",
			NULL,
			NULL,
			(LPBYTE)&cbVal,
			&cbData
		);

		if (dwRet == ERROR_SUCCESS)
			cout << "\nValue of Something is " << cbVal << endl;
		else cout << "\nRegQueryValueEx failed " << dwRet << endl;
	}
	else
	{
		cout << "RegOpenKeyEx failed " << lRv << endl;
	}
}

int main()
{
	int wait;
	HKEY hKey;
	LSTATUS status;
	status = RegCreateKeyEx(HKEY_CURRENT_USER, L"Software\\Onstream\\testonly\\3.0\\License", 0, NULL,
		REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL);
	if (status == ERROR_SUCCESS)
		printf("success \n");  
	else printf("%u\n", status);
	RegCloseKey(hKey);

	writeToRegistry();
	readValueFromRegistry();

	cin >> wait;
	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
