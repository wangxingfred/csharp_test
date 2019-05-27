//**************************************************************************************
//Create By fred on 2019/04/28
//
//@Description 测试继承对构造函数的影响
//**************************************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleApplication.Inherit
{
    abstract class Base
    {

        protected int a = -99;

        protected Base() : this(0)
        {
            a++;
            Console.WriteLine($"Base 0 : a = {a}");
        }

        protected Base(int i)
        {
            a = i;
            Console.WriteLine($"Base 1 : a = {a}");

            Init();
        }

        protected abstract void Init();
    }

    class Child : Base
    {
        protected int b = -88;

        internal Child()
        {
            Console.WriteLine($"Child 0 : a = {a}");
        }

        internal Child(int i) : base(i)
        {
            Console.WriteLine($"Child 1 : a = {a}");
        }

        protected override void Init()
        {
            Console.WriteLine($"Child Init : b = {b}");
            Console.WriteLine($"Child Init : a = {a}");
        }
    }

    class InheritConstructor : ITest
    {
        void ITest.Run()
        {
            Base a = new Child();
        }
    }
}
