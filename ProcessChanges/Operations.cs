using System.Configuration;
using System.Data.SqlClient;

namespace ProcessChanges
{
    public static class Operations
    {
        private const string listenCmd = "SELECT * FROM [dbo].[RemoteEvents] where [Processed]=1";
        public static string ConnectionString;
        public static void StartSqlDependency()
        {
            ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            SqlDependency.Start(ConnectionString);
        }

        public static void SubscribeToEvents()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(
                    listenCmd,connection ))
                {
                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new
                       OnChangeEventHandler(OnDependencyChange);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Process the DataReader.
                    }
                }
            }
        }

        // Handler method
        public static void OnDependencyChange(object sender,
           SqlNotificationEventArgs e)
        {
            System.Console.WriteLine(e.Info);
        }

        public static void Termination()
        {
            // Release the dependency.
            SqlDependency.Stop(ConnectionString);
        }
    }
}
