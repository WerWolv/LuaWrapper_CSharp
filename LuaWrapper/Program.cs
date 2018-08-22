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
        public static object[] lua_test(string i, double j)
        {
            Console.WriteLine($"lua_test called! {i[0]}, {j}");

            return new object[] { "Test1", "Test2" };
        }

        static void Main(string[] args)
        {
            LuaContext context = new LuaContext();
            context.LoadFromFile("test.lua");
            context.RegisterFunction("edizon", "test", new Func<string, double, object[]>(lua_test));
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
