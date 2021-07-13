// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using RestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackTeamsChanges
{
    public class DbOperations
    {
        public static void AddTeams(IList<Teams> teams)
        {
            using (var context = new DbCtxt())
            {
                context.Teams.AddRange(teams);
                context.SaveChanges();
            }
        }
        public static void AddTeams(Teams teams)
        {
            using (var context = new DbCtxt())
            {
                context.Teams.Add(teams);
                context.SaveChanges();
            }
        }
        public static void AddSubscriptions(IList<Subscription> subscriptions)
        {
            using (var context = new DbCtxt())
            {
                context.Subscriptions.AddRange(subscriptions);
                context.SaveChanges();
            }
        }
        public static void AddSubscriptions(Subscription subscriptions)
        {
            using (var context = new DbCtxt())
            {
                var sub = context.Subscriptions.Where(w => w.SubscriptionId == subscriptions.SubscriptionId).FirstOrDefault();
                if (sub == null)
                    context.Subscriptions.Add(subscriptions);
                context.SaveChanges();
            }
        }
        public static bool IsSubscriptionCreatedForTeam(string teamId)
        {
            using (var context = new DbCtxt())
            {
                var sub = context.Subscriptions.Where(w => w.TeamId == teamId).FirstOrDefault();
                if (sub == null)
                    return false;
                else
                    return true;
            }
        }
        public static void AddNotifications(IList<SPWebhookNotification> notifications)
        {
            using (var context = new DbCtxt())
            {
                context.SPWebhookNotifications.AddRange(notifications);
                context.SaveChanges();
            }
        }
        public static void AddNotifications(SPWebhookNotification notifications)
        {
            using (var context = new DbCtxt())
            {
                context.SPWebhookNotifications.Add(notifications);
                context.SaveChanges();
            }
        }
        public static IList<Teams> GetTeams(int limit)
        {
            using (var context = new DbCtxt())
            {
                return context.Teams.OrderBy(o => o.CreatedOn).Take(limit).ToList();
            }
        }

        internal static void ClearTeamsTable()
        {
            using (var context = new DbCtxt())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Teams");
            }
        }

        public static void DeleteSubscription(Subscription subscription)
        {
            using (var context = new DbCtxt())
            {
                var sub = context.Subscriptions.Where(w => w.SubscriptionId == subscription.SubscriptionId).FirstOrDefault();
                if (sub == null)
                    return;
                    context.Subscriptions.Remove(sub);
                    context.SaveChanges();
            }
        }
    }
}
