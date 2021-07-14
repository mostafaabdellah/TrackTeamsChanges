using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphApi.Authentication;
using TrackTeamsChanges;
using RestApi;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Threading;

namespace GraphApi.Services
{
    public static class Operations
    {
        private const string ClientState = "A0A354EC-97D4-4D83-9DDB-144077ADB449";
        private static string AccessToken = string.Empty;
        private const string NotificationUrl = "https://0284f0573ffb.ngrok.io/api/graphwebhook/handlerequest";

        readonly static DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider();
        readonly static GraphServiceClient graphClient = new GraphServiceClient(authProvider);
        public static void CreateSubscriptions(int count)
        {
            AccessToken = authProvider.GetAccessToken().Result;
            DbOperations.GetTeams(count)
                .ToList().ForEach(team => {
                    CreateSubscriptionAsync(team).Wait();
                });
        }
        public static void DeleteDriveContent(int count)
        {
            var counter = 4000;
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var teams = DbOperations.GetTeams(count)
                .Skip(counter).Take(2005).ToList();
            Parallel.ForEach(teams, options, team =>
            {
                DeleteDriveContentAsync(team).Wait();
                Console.WriteLine(counter++);
            });
        }
        public static void CreateGeneralFolder(int count)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var teams = DbOperations.GetTeams(count)
                .ToList();

            Parallel.ForEach(teams, options, team =>
            //teams.ForEach(team=>
            {
                bool result = CreateFolderAsync(team, "General").Result;
                if (result)
                {
                    DbOperations.AddChange(team.TeamId, 1, "FolderCreated");
                    Console.WriteLine($"{team.SiteUrl} General Folder Created");
                }
            });
        }
        public static void UploadFileToGeneralFolder(int count)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var teams = DbOperations.GetTeams(count)
                .ToList();
            Parallel.ForEach(teams, options, team =>
            {
                UploadSmallFile(team, "/General/test.txt","Test Document").Wait();
                DbOperations.AddChange(team.TeamId, 2, "FileUploaded");
            });
        }
        public static void UpdateFileToGeneralFolder(int count)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var teams = DbOperations.GetTeams(count)
                .ToList();
            Parallel.ForEach(teams, options, team =>
            {
                UploadSmallFile(team, "/General/test.txt", "Test Document1").Wait();
                DbOperations.AddChange(team.TeamId, 3, "FileUploaded");
            });
        }
        private static async Task UploadSmallFile(Teams team, string path, string fileContent)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(fileContent);
                    writer.Flush();
                    stream.Position = 0;

                    var createdFile = await graphClient.Drives[team.DriveId]
                                .Root
                                .ItemWithPath($"{path}")
                                .Content.Request()
                                .PutAsync<DriveItem>(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static async Task<bool> CreateFolderAsync(Teams team, string FolderName)
        {
            var driveItem = new DriveItem
            {
                Name = FolderName,
                Folder = new Folder
                {

                },
                AdditionalData = new Dictionary<string, object>()
                {
                    { "@microsoft.graph.conflictBehavior", "rename"}//rename | fail | replace
                }
            };
            try
            {

                var s=await graphClient.Drives[team.DriveId].Root.Children
                    .Request()
                    .AddAsync(driveItem);
                return true;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                Thread.Sleep(10000);
                return false;
            }
        }
        
        private static async Task DeleteDriveContentAsync(Teams team)
        {
            try
            {
                var items = await graphClient.Drives[team.DriveId]
                    .Root
                    .Children
                    .Request()
                    .GetAsync();
                foreach (var item in items)
                {
                    await graphClient.Drives[team.DriveId]
                        .Items[item.Id]
                        .Request()
                        .DeleteAsync();
                    //Console.WriteLine($"{item.WebUrl} - {item.Name} Deleted");
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(10000);
            }
        }

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
        public static async Task DeleteSubscriptions()
        {
            var subscriptions = await graphClient.Subscriptions
                .Request()
                .GetAsync();

            var filtered = subscriptions.CurrentPage.Where(w =>
                        w.ClientState == ClientState);

            DeleteSubscriptions(filtered);


            while (subscriptions.AdditionalData.ContainsKey("@odata.nextLink")
                    && subscriptions.AdditionalData["@odata.nextLink"] != null)
            {
                var nextLink = subscriptions.AdditionalData["@odata.nextLink"].ToString();
                subscriptions.InitializeNextPageRequest(graphClient, nextLink);
                subscriptions = await subscriptions.NextPageRequest
                    .GetAsync();

                filtered = subscriptions.CurrentPage.Where(w =>
                        w.ClientState == ClientState);

                DeleteSubscriptions(filtered);
            }

        }

        private static void DeleteSubscriptions(IEnumerable<Microsoft.Graph.Subscription> Subscriptions)
        {
            Subscriptions.ToList().ForEach(subscription => {
                var response=graphClient.Subscriptions[subscription.Id]
                .Request()
                .DeleteAsync();
                DbOperations.DeleteSubscription(subscription.Id);
            });
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
                teams.Add(new Teams()
                {
                    TeamId = team.Id,
                    DisplayName= team.DisplayName,
                    CreatedOn= team.CreatedDateTime.Value.UtcDateTime,
                    DriveId= teamSite.ParentReference.DriveId,
                    SiteUrl = teamSite.WebUrl,
                    SiteId=teamSite.SharepointIds.SiteId,
                    ListId = teamSite.SharepointIds.ListId,

                });
                Console.WriteLine($"{teamSite.WebUrl} Added");
                DbOperations.AddTeams(teams);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }
        private static void CreateSubscription(Teams team)
        {
            try
            {
                DateTime dateTime = DateTime.UtcNow + new TimeSpan(0, 0, 4200, 0);
                var subscription = graphClient.Subscriptions.Request().AddAsync(new Microsoft.Graph.Subscription
                {
                    Resource = $"/drives/{team.DriveId}/root",
                    ChangeType = "updated",
                    ExpirationDateTime = dateTime.ToUniversalTime(),
                    NotificationUrl = NotificationUrl
                }).Result;

                Console.WriteLine($"{team.SiteUrl} Added - subscription.Id {subscription.Id}");
                DbOperations.AddSubscriptions(new Subscription() { 
                    ClientState=subscription.ClientState,
                    ExpirationDateTime=subscription.ExpirationDateTime.Value.UtcDateTime.ToString(),
                    SubscriptionId=subscription.Id,
                    Resource=subscription.Resource,
                    TeamId=team.TeamId,
                    NotificationUrl=subscription.NotificationUrl,
                    Type=subscription.ChangeType,
                    Oid=subscription.ApplicationId,
                    EditLink=team.SiteUrl
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static async Task CreateSubscriptionAsync(Teams team)
        {
            if (DbOperations.IsSubscriptionCreatedForTeam(team.TeamId))
                return;
            var url = "https://graph.microsoft.com/beta/subscriptions";
            SubscriptionPostGraph post = new SubscriptionPostGraph()
            {
                Resource = $"/drives/{team.DriveId}/root",
                NotificationUrl = NotificationUrl,
                ClientState = ClientState,
                ExpirationDateTime="2021-08-05T11:00:00.0000000Z"
            };

            var payload = JsonConvert.SerializeObject(post, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(url, content);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                var subscription = JsonConvert.DeserializeObject<SubscriptionGraph>(resultContent);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{team.SiteUrl} Added - subscription.Id {subscription.SubscriptionId}");
                DbOperations.AddSubscriptions(new Subscription()
                {
                    ClientState = subscription.ClientState,
                    ExpirationDateTime = subscription.ExpirationDateTime,
                    SubscriptionId = subscription.SubscriptionId,
                    Resource = subscription.Resource,
                    TeamId = team.TeamId,
                    NotificationUrl = subscription.NotificationUrl,
                    Type = subscription.ChangeType,
                    Oid = subscription.ApplicationId,
                    EditLink=team.SiteUrl
                });
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(resultContent);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (resultContent.Contains("Access token has expired"))
                {
                    AccessToken = authProvider.GetAccessToken().Result;
                    await CreateSubscriptionAsync(team);
                    Console.WriteLine("retry successed");
                }
            }

        }
    }
}
