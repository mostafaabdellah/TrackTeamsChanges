using GraphApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");
            //Operations.UpdateTeamsTable().Wait();
            //Operations.DeleteDriveContent(6666);
            Operations.CreateGeneralFolder(1000);
            Console.WriteLine("Job Completed Exit!");
            Console.ReadLine();
        }
    }
}
