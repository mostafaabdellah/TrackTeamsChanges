using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackTeamsChanges.Authentication;

namespace TrackTeamsChanges.Services
{
    public static class Operations
    {
        readonly static DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider();
        readonly static GraphServiceClient graphClient = new GraphServiceClient(authProvider);

        public static async Task UpdateTeamsTable()
        {
            var teams = await graphClient.Groups
                               .Request()
                               .Filter("resourceProvisioningOptions/Any(x:x eq 'Team')")
                               .Select("id,displayName,CreatedDateTime")
                               .GetAsync();
            AddTeamsToTable(teams);


            while (teams.AdditionalData.ContainsKey("@odata.nextLink")
                    && teams.AdditionalData["@odata.nextLink"] != null)
            {
                var nextLink = teams.AdditionalData["@odata.nextLink"].ToString();
                teams.InitializeNextPageRequest(graphClient, nextLink);
                teams = await teams.NextPageRequest
                    .GetAsync();
                AddTeamsToTable(teams);
            }

        }
        private static void AddTeamsToTable(IGraphServiceGroupsCollectionPage teams)
        {
        //    var options = new ParallelOptions()
        //    {
        //        MaxDegreeOfParallelism = Environment.ProcessorCount
        //    };
        //    Parallel.ForEach(teams, options, async team =>
        //    {
        //        await AddTeamToTable(team);
        //    });
            foreach (var team in teams)
                AddTeamToTable(team).Wait();

    }
        private static async Task AddTeamToTable(Group team)
        {
            try
            {
                var teamSite = await graphClient.Groups[team.Id].Drive.Root
                                        .Request()
                                        .Select("ParentReference,CreatedDateTime,WebUrl")
                                        .GetAsync();
                var teams = new List<Teams>();
                var driveId = teamSite.ParentReference.DriveId;
                var subscription= graphClient.Subscriptions.Request().AddAsync(new Subscription
                {
                    Resource=$"/drives/{driveId}/root",
                    ChangeType="updated",
                    ExpirationDateTime= DateTime.UtcNow + new TimeSpan(0, 0, 4200, 0),
                    NotificationUrl= "https://40a818a4b0e0.ngrok.io/api/spwebhook/handlerequest"
                }).Result;
                teams.Add(new Teams()
                {
                    TeamId = team.Id,
                    DisplayName= team.DisplayName,
                    CreatedOn= team.CreatedDateTime.Value.UtcDateTime,
                    DriveId= driveId,
                    SiteUrl= teamSite.WebUrl,
                    SubscriptionId=subscription.Id,
                    SubscriptionExpirationDate=subscription.ExpirationDateTime.Value.UtcDateTime
                });
                Console.WriteLine($"subscription {subscription.Id} add to {teamSite.WebUrl}");
                await DbOperations.AddTeamsToTable(teams);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }
    }
}
