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
                .Delta()
                .Request()
                .Select("id,displayName,visibility,resourceProvisioningOptions,CreatedDateTime")
                .GetAsync();

            var teamsFiltered = teams.CurrentPage.Where(w =>
                        w.ResourceProvisioningOptions != null
                    && w.ResourceProvisioningOptions.Contains("Team"));

            AddTeamsToTable(teamsFiltered);


            while (teams.AdditionalData.ContainsKey("@odata.nextLink")
                    && teams.AdditionalData["@odata.nextLink"] != null)
            {
                var nextLink = teams.AdditionalData["@odata.nextLink"].ToString();
                teams.InitializeNextPageRequest(graphClient, nextLink);
                teams = await teams.NextPageRequest
                    .GetAsync();

                teamsFiltered = teams.CurrentPage.Where(w =>
                        w.ResourceProvisioningOptions != null
                    && w.ResourceProvisioningOptions.Contains("Team"));

                AddTeamsToTable(teamsFiltered);
            }

        }

        private static void AddTeamsToTable(IEnumerable<Group> teams)
        {
            //var options = new ParallelOptions()
            //{
            //    MaxDegreeOfParallelism = Environment.ProcessorCount
            //};
            //Parallel.ForEach(teams, options, async team =>
            //{
            //    await AddTeamToTable(team);
            //});

            foreach (var team in teams)
                AddTeamToTable(team).Wait();

        }
        private static async Task AddTeamToTable(Group team)
        {
            try
            {
                var teamSite = await graphClient.Groups[team.Id].Drive.Root
                                        .Request()
                                        .Select("SharepointIds,ParentReference,CreatedDateTime,WebUrl")
                                        .GetAsync();
                var teams = new List<Teams>();
                //var driveId = teamSite.ParentReference.DriveId;
                //var subscription = graphClient.Subscriptions.Request().AddAsync(new Subscription
                //{
                //    Resource = $"/drives/{driveId}/root",
                //    ChangeType = "updated",
                //    ExpirationDateTime = DateTime.UtcNow + new TimeSpan(0, 0, 4200, 0),
                //    NotificationUrl = "https://d8a4b80a98a5.ngrok.io/api/spwebhook/handlerequest"
                //}).Result;
                teams.Add(new Teams()
                {
                    TeamId = team.Id,
                    DisplayName= team.DisplayName,
                    CreatedOn= team.CreatedDateTime.Value.UtcDateTime,
                    //DriveId= driveId,
                    SiteUrl= teamSite.WebUrl,
                    SiteId=teamSite.SharepointIds.SiteId,
                    ListId = teamSite.SharepointIds.ListId,
                    //SubscriptionId =subscription.Id,
                    //SubscriptionExpirationDate=subscription.ExpirationDateTime.Value.UtcDateTime
                });
                //Console.WriteLine($"{teamSite.WebUrl} Added - subscription.Id {subscription.Id}");
                Console.WriteLine($"{teamSite.WebUrl} Added");
                await DbOperations.AddTeamsToTable(teams);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }
    }
}
