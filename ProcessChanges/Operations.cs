using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ProcessChanges
{
    public class Operations
    {
        public static string sampleConnectionString= ConfigurationManager.ConnectionStrings["TrackTeamsChanges"].ConnectionString;
        private SqlDependency sampleSqlDependency;
        private SqlCommand sampleSqlCommand;
        private SqlConnection sampleSqlConnection;
        private string notificationQuery;

        public void StartSqlDependency()
        {
            SqlDependency.Stop(sampleConnectionString);
            SqlDependency.Start(sampleConnectionString);
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
                Console.WriteLine("Notification Info: " + eventArgs.Info);
                Console.WriteLine("Notification source: " + eventArgs.Source);
                Console.WriteLine("Notification type: " + eventArgs.Type);
            }
            ConfigureDependencyUsingTextQueryAndDefaultQueue();
        }

    }
}
