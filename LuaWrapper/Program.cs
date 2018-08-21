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
        static int lua_test(IntPtr l)
        {
            Console.WriteLine("lua_test called!");

            NativeCalls.lua_pushnumber(l, 69);

            return 1;
        }

        static void Main(string[] args)
        {           
            LuaContext context = new LuaContext();
            context.LoadFromFile("test.lua");
            context.RegisterFunction("test", lua_test);
            context.Execute();

            object[] ret = context.Execute("lol");

            Console.ReadKey();

        }
    }
}
