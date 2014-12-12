// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Threading;
using System.Threading.Tasks;
using Weather.Utils;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace Weather.Tasks
{ 
   public sealed class UpdateTileTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            UpdateTile();

            //表示完成任务
            _deferral.Complete();
        }


        public void UpdateTile()
        {
            var tileModel = new
            {
                ImagerSrc = "ms-appx:///Assets/Logo.scale-100.png",
                TextHeading = "气温 17",
                TextBody1 = "风向 东北风",
                TextBody2 = "风力 2级",
                TextBody3 = "湿度 4%"
            };

            TileHelper.UpdateTileNotifications(tileModel.ImagerSrc, tileModel.TextHeading, tileModel.TextBody1, tileModel.TextBody2, tileModel.TextBody3);
            TileHelper.UpdateBadgeWithNumber(10);
        }
    }
}
