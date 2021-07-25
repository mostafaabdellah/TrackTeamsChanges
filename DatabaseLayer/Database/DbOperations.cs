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
        public static bool IsGraphSubscriptionCreatedForTeam(string teamId)
        {
            using (var context = new DbCtxt())
            {
                var sub = context.Subscriptions.Where(w => w.TeamId == teamId 
                && w.Resource.Contains("drives")).FirstOrDefault();
                if (sub == null)
                    return false;
                else
                    return true;
            }
        }

        public static void AddLog(LogInfo logInfo)
        {
            using (var context = new DbCtxt())
            {
                context.LogInfo.Add(logInfo);
                context.SaveChanges();
            }
        }

        public static void AddChange(string teamId, int Iteration, string ChangeType)
        {
            using (var context = new DbCtxt())
            {
                context.Changes.Add(new Change()
                {
                    TeamId=teamId,
                    Iteration=Iteration,
                    ChangeType=ChangeType,
                    CreatedOn=DateTime.UtcNow
                });
                context.SaveChanges();
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
        public static void AddNotifications(SPWebhookNotification notification)
        {
            using (var context = new DbCtxt())
            {
                context.SPWebhookNotifications.Add(notification);
                var subscription = context.Subscriptions
                    .Where(w => w.SubscriptionId == notification.SubscriptionId)
                    .FirstOrDefault();
                if (subscription != null)
                {
                    var change = context.Changes
                        .Where(w => w.TeamId == subscription.TeamId)
                        .OrderByDescending(o=>o.Iteration)
                        .FirstOrDefault();
                    if(change!=null)
                    {
                        if(notification.Resource.Contains("drives"))
                        {
                            change.GraphSubscriptionId = notification.SubscriptionId;
                            change.GraphNotificationOn = DateTime.UtcNow;
                            change.GraphLatency = (int)(DateTime.UtcNow - change.CreatedOn).TotalSeconds;
                        }
                        else
                        {
                            change.RestSubscriptionId = notification.SubscriptionId;
                            change.RestNotificationOn = DateTime.UtcNow;
                            change.RestLatency = (int)(DateTime.UtcNow - change.CreatedOn).TotalSeconds;
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        public static IList<RemoteEvent> GetLatestEvents()
        {
            using (var context = new DbCtxt())
            {

                var items=context.RemoteEvents.Where(w => w.Processed == null).ToList();
                items.ForEach(i => {
                    i.Processed = true;
                });
                context.SaveChanges();
                return items;
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
        public static void DeleteSubscription(string SubscriptionId)
        {
            using (var context = new DbCtxt())
            {
                var sub = context.Subscriptions.Where(w => w.SubscriptionId == SubscriptionId).FirstOrDefault();
                if (sub == null)
                    return;
                context.Subscriptions.Remove(sub);
                context.SaveChanges();
            }
        }

        public static void AddRemoteEventReceiver(RemoteEventReceiver rer)
        {
            using (var context = new DbCtxt())
            {
                context.RemoteEventReceivers.Add(rer);
                context.SaveChanges();
            }
        }

        public static void AddRemoteEvent(RemoteEvent re)
        {
            using (var context = new DbCtxt())
            {
                context.RemoteEvents.Add(re);
                context.SaveChanges();
            }
        }
    }
}
