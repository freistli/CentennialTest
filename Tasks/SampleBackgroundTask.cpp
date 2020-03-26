//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// SampleBackgroundTask.cpp
#include "pch.h"
#include "SampleBackgroundTask.h"
#include "winstring.h"

using namespace Tasks;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::System::Threading;
using namespace CXStoreEngagementSDKFeatures;
using namespace Windows::UI::Notifications;
using namespace Microsoft::Services::Store::Engagement;
using namespace Platform;
using namespace concurrency;

BackgroundTaskDeferral^ _deferral;
void SampleBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
	/*
	_deferral = taskInstance->GetDeferral();	
	
	ToastNotificationActionTriggerDetail^ details = (ToastNotificationActionTriggerDetail^)taskInstance->TriggerDetails;

	if (details )
	{
		StoreServicesEngagementManager^ engagementManager = StoreServicesEngagementManager::GetDefault();
		String^ toast = engagementManager->ParseArgumentsAndTrackAppLaunch(details->Argument);

		// Use the originalArgs variable to access the original arguments
		// that were passed to the app.
	}
	_deferral->Complete();
	*/
}

//
// Handles background task cancellation.
//
void SampleBackgroundTask::OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason)
{
    //
    // Indicate that the background task is canceled.
    //
    CancelRequested = true;
    CancelReason = reason;
}
