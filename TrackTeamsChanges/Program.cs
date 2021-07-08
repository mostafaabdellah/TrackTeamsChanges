using System;
using TrackTeamsChanges.Services;

namespace TrackTeamsChanges
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");
            Operations.UpdateTeamsTable().Wait();
        }
    }
}
