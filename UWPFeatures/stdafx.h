#pragma once

#include "targetver.h"
#include <stdio.h>
#include <tchar.h>
#using <Windows.winmd>
#using <CXStoreEngagementSDKFeatures.winmd>
#include <iostream>
#include <string>
#include <ppltasks.h>
#include <windows.h>
#include <appmodel.h>
#include <malloc.h>
#include <stdio.h>
#include <assert.h>
#define UWPFeatures_API __declspec(dllexport)

extern "C"
{
	UWPFeatures_API PCWSTR AppDataPath();
	UWPFeatures_API PCWSTR RegisterBackgroundTask();
	UWPFeatures_API void  WinInitialize();
	UWPFeatures_API void  RegisterStoreNotification();
}
