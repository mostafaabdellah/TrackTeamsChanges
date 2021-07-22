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

        public DbCtxt():base(@"Server=CSMM1\WIN19SQL17CI;Database=TrackTeamsChanges;Trusted_Connection=True;")
        {
            _ =
                System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
