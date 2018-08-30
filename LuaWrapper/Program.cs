using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaWrapper
{
    class Program
    {
        static (int, int) multRet(string s, int i)
        {
            Console.WriteLine(s);
            return (i, i + 2);
        }

        static void Main(string[] args)
        {
            LuaContext context = new LuaContext();
            context.LoadFromFile("test.lua");

            context.RegisterFunction("", "emptyRet", new Action<int>((int i) => { Console.WriteLine(i); }));
            context.RegisterFunction("edizon", "oneRet", new Func<string, string>((string i) => { return i; }));
            context.RegisterFunction("edizon", "multRet", new Func<string, int, (int, int)>(multRet));

            context.Execute();

            Console.ReadKey();

        }

        public static double lol(int i, int j)
        {
            return 5;
        }

        public static void test(Delegate d)
        {
            var i = d.Method.GetParameters();
        }
    }
}
