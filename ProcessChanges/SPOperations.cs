using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TrackTeamsChanges;

namespace ProcessChanges
{
    public class SPOperations
    {
        public static string sampleConnectionString= ConfigurationManager.ConnectionStrings["TrackTeamsChanges"].ConnectionString;
        public static bool ShowDetails = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowDetails"]);
        private SqlDependency sampleSqlDependency;
        private SqlCommand sampleSqlCommand;
        private SqlConnection sampleSqlConnection;
        private string notificationQuery;

        public void StartSqlDependency()
        {
            //SqlDependency.Stop(sampleConnectionString);
            //SqlDependency.Start(sampleConnectionString);
            notificationQuery = "SELECT [Id],[Processed] FROM [dbo].[RemoteEvents] where [Processed] is null;";
            ConfigureDependencyUsingTextQueryAndDefaultQueue();
        }
        private async void ConfigureDependencyUsingTextQueryAndDefaultQueue()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            //// Create connection.
            this.sampleSqlConnection = new SqlConnection(sampleConnectionString);

            //// Create command.
            this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
            this.sampleSqlCommand.CommandType = CommandType.Text;
            this.sampleSqlCommand.CommandText = this.notificationQuery;
            this.sampleSqlCommand.Notification = null;

            //// Create Sql Dependency.
            this.sampleSqlDependency = new SqlDependency(this.sampleSqlCommand);
            this.sampleSqlDependency.OnChange += this.SqlDependencyOnChange;
            await this.sampleSqlCommand.Connection.OpenAsync();
            await this.sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }
        }
        private void SqlDependencyOnChange(object sender, SqlNotificationEventArgs eventArgs)
        {
            if (eventArgs.Info == SqlNotificationInfo.Invalid)
            {
                Console.WriteLine("The above notification query is not valid.");
            }
            else 
            {
                //Console.WriteLine("Notification Info: " + eventArgs.Info);
                if (eventArgs.Info == SqlNotificationInfo.Insert)
                    HandleNewEvent();
            }
            ConfigureDependencyUsingTextQueryAndDefaultQueue();
        }

        private void HandleNewEvent()
        {
            var events = DbOperations.GetLatestEvents();
            events.ToList().ForEach(e=> 
            {
                ProcessEvent(e);
            });
        }

        private void ProcessEvent(RemoteEvent e)
        {
            switch ((RemoteEventType)e.EventType)
            {
                case RemoteEventType.ItemAdded:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"New Item Added {e.WebUrl}/{e.AfterUrl}");
                    break;
                case RemoteEventType.ItemUpdated:
                    Console.ForegroundColor = ConsoleColor.White;
                    if (e.BeforeUrl != e.AfterUrl)
                        Console.WriteLine($"Item Renamed {e.WebUrl} from {e.BeforeUrl} to {e.AfterUrl}");
                    else if (IsContentChanged(e))
                        Console.WriteLine($"Content Changed {e.WebUrl}/{e.BeforeUrl}");
                    //else 
                    //    Console.WriteLine($"Item Updated {e.WebUrl}/{e.BeforeUrl}");
                    break;
                case RemoteEventType.ItemDeleted:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Item Deleted {e.WebUrl}/{e.BeforeUrl}");
                    break;
                case RemoteEventType.ItemCheckedIn:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Item CheckedIn {e.WebUrl}/{e.AfterUrl}");
                    break;
                case RemoteEventType.ItemCheckedOut:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Item CheckedOut {e.WebUrl}/{e.AfterUrl}");
                    break;
                case RemoteEventType.ItemUncheckedOut:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Item UncheckedOut {e.WebUrl}/{e.AfterUrl}");
                    break;
                case RemoteEventType.ItemFileMoved:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Item Moved from {e.WebUrl}/{e.BeforeUrl} to {e.AfterUrl}");
                    break;
            }
            if(ShowDetails)
            {
                Console.WriteLine($"\nWebUrl={e.WebUrl}\n");
                Console.WriteLine($"EventType={(RemoteEventType)e.EventType}\n");
                Console.WriteLine($"ListId={e.ListId}\n");
                Console.WriteLine($"ListItemId={e.ListItemId}\n");
                Console.WriteLine($"UserLoginName={e.UserLoginName}\n");
                Console.WriteLine($"BeforeUrl={e.BeforeUrl}\n");
                Console.WriteLine($"AfterUrl={e.AfterUrl}\n");
                Console.WriteLine($"BeforeProperties={e.BeforeProperties}\n");
                Console.WriteLine($"AfterProperties={e.AfterProperties}\n");
                Console.WriteLine($"----------------------------------------------\n");
            }
        }

        private bool IsContentChanged(RemoteEvent e)
        {
            var before= JsonConvert.DeserializeObject<Dictionary<string, string>>(e.BeforeProperties);
            var after = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.AfterProperties);
            if (before["vti_writevalidationtoken"] == "AAAAAAAAAAAAAAAAAAAAAAAAAAA=")
                return false;
            if (before["vti_writevalidationtoken"] != after["vti_writevalidationtoken"])
                return true;
            return false;
        }
    }
}
