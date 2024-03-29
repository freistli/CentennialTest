// UWPFeatures.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
using namespace std;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::System;
using namespace Windows::Storage; 
using namespace concurrency;
using namespace Windows::ApplicationModel::Background;
using namespace CXStoreEngagementSDKFeatures;

LPCWSTR RegisterBackgroundTask()
{
	create_task(BackgroundExecutionManager::RequestAccessAsync()).then([=](BackgroundAccessStatus b) {
		String^ triggerName = L"UniqueTrigger";
		BackgroundTaskBuilder^ builder = ref new BackgroundTaskBuilder();
		builder->Name = triggerName;
		builder->SetTrigger(ref new SystemTrigger(SystemTriggerType::TimeZoneChange, false));
		builder->TaskEntryPoint = "Tasks.SampleBackgroundTask";
		builder->Register();
	}).then([]() {
		Launcher::LaunchUriAsync(ref new Uri(L"https://blogs.windows.com/buildingapps//"));
	});
	return (ref new String( L"Registered\n"))->Data();
}
void RegisterStoreNotification()
{
	Wrapper^ wp = ref new Wrapper();
	wp->RegisterService();
}
LPCWSTR AppDataPath()
{	
	return (ApplicationData::Current->LocalCacheFolder->Path +L"\\Roaming\n")->Data();	 
}
void WinInitialize()
{
	Windows::Foundation::Initialize();
}