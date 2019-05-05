using System;
using System.Collections.Generic;
using ConsoleApplication;

namespace ConsoleApplication.Dictionary
{
    class PricedCachePool_Test : ITest
    {
        void ITest.Run()
        {
            var capacity = 5;
            var replicaLimit = 2;
            var pool = new PricedCachePool<string, string>(capacity, replicaLimit);

            string value = "";

            pool.TryTake("1", out value);

            pool.TryPut("2", "b", 2);
            pool.TryPut("3", "c", 1);
            pool.TryPut("5", "e", 5);
            pool.TryPut("7", "a", 1);
            pool.TryPut("4", "d", 4);
            pool.TryPut("6", "f", 3);

            pool.TryPut("1", "aa", 10);
            pool.TryPut("2", "bb", 20);
            pool.TryPut("3", "cc", 30);
        }
    }
}
