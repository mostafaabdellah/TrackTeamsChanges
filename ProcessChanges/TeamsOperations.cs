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
    public class TeamsOperations
    {
        public static string sampleConnectionString= ConfigurationManager.ConnectionStrings["TrackTeamsChanges"].ConnectionString;
        public static bool ShowDetails = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowDetails"]);
        private SqlDependency sampleSqlDependency;
        private SqlCommand sampleSqlCommand;
        private SqlConnection sampleSqlConnection;
        private string notificationQuery;

        public void StartSqlDependency()
        {
            SqlDependency.Stop(sampleConnectionString);
            SqlDependency.Start(sampleConnectionString);
            notificationQuery = "SELECT [TeamId],[CreatedOn],[DriveId] FROM [dbo].[Teams] where [DriveId] is null;";
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
            var events = DbOperations.GetLatestTeams();
            events.ToList().ForEach(e=> 
            {
                ProcessEvent(e);
            });
        }

        private void ProcessEvent(Teams e)
        {
            var team=GraphApi.Services.Operations.GetTeamsDetails(e.TeamId).Result;
            Console.WriteLine($"\nNew Team Created {team.DisplayName} TeamId={e.TeamId}\n");
            DbOperations.UpdateTeams(team);
            for (int i = 1; i <= 10; i++)
                RestApi.Operations.CreateTeamsRER(team, i).Wait();

        }

    }
}
