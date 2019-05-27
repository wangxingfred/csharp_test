using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            ITest[] tests =
            {
                //new Dictionary.PricedCachePool_Test()
                new Inherit.InheritConstructor()
            };


            foreach (var t in tests)
            {
                t.Run();
            }


        }
    }
}
