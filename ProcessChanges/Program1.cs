using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessChanges
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create Teams Subscription...");
            //GraphApi.Services.Operations.CreateTeamsSubscriptionAsync().Wait();
            Console.WriteLine("Start listening to events...");
            var teamsOprations = new TeamsOperations();
            teamsOprations.StartSqlDependency();
            var spOprations = new SPOperations();
            spOprations.StartSqlDependency();
            Console.ReadLine();
        }
    }
}
