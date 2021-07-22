// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackTeamsChanges
{
    public class Teams
    {
        [Key]
        public string TeamId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string DriveId { get; set; }
        public string DisplayName { get; set; }
        public string SiteUrl { get; set; }
        public string SiteId { get; set; }
        public string ListId { get; set; }
    }
}
public class LogInfo
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ResponseContent { get; set; }
    public string Exception { get; set; }

}
public class EventReceiver
{
    public int EventType { get; set; }
    public string ReceiverAssembly { get; set; } = "Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
    public string ReceiverClass { get; set; } = "Microsoft.SharePoint.Webhooks.SPWebhookItemEventReceiver";
    public string ReceiverName { get; set; }
    public string ReceiverUrl { get; set; }
    public int SequenceNumber { get; set; } = 10000;
    public int Synchronization { get; set; } = 2;
}