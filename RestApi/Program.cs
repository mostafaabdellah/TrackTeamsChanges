using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Operations.CreateTeamsRERs(0,6000);
            //Operations.CreateSubscriptions(0,1);
            //Operations.DeleteSubscriptions(6666);
            Console.WriteLine("Job Completed Exit!");
            Console.ReadLine();
        }

        
    }
}
