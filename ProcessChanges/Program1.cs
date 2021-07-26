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
            var op = new Operations();
            op.StartSqlDependency();
            Console.WriteLine("Start listening to events...");
            Console.ReadLine();
        }
    }
}
