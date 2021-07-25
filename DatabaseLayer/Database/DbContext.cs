// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using RestApi;
using System.Data.Entity;
using WebhookReceiver.Models;

namespace TrackTeamsChanges
{
    public class DbCtxt : DbContext
    {
        public DbSet<Teams> Teams { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SPWebhookNotification> SPWebhookNotifications { get; set; }
        public DbSet<Change> Changes { get; set; }
        public DbSet<LogInfo> LogInfo { get; set; }
        public DbSet<RemoteEventReceiver> RemoteEventReceivers { get; set; }
        public DbSet<RemoteEvent> RemoteEvents { get; set; }
        //public DbSet<REREvent> REREvents { get; set; }

        public DbCtxt():base("name=TrackTeamsChanges")
        {
            _ =
                System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
