using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaWrapper
{
    public delegate int lua_CFunction(IntPtr L);

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct luaL_Reg
    {   public IntPtr name;

        public IntPtr func;
    }

    public static class NativeCalls
    {
        public const int LUA_TNONE          = -1;
        public const int LUA_TNIL           =  0;
        public const int LUA_TBOOLEAN       =  1;
        public const int LUA_TLIGHTUSERDATA =  2;
        public const int LUA_TNUMBER        =  3;
        public const int LUA_TSTRING        =  4;
        public const int LUA_TTABLE         =  5;
        public const int LUA_TFUNCTION      =  6;
        public const int LUA_TUSERDATA      =  7;
        public const int LUA_TTHREAD        =  8;
        public const int LUA_NUMTAGS        =  9;


        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_newstate")]
        public static extern IntPtr luaL_newstate();

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_openlibs")]
        public static extern void luaL_openlibs(IntPtr L);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_loadfilex")]
        public static extern int luaL_loadfilex(IntPtr L, string filename, string mode);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_loadstring")]
        public static extern int luaL_loadstring(IntPtr L, string s);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pcallk")]
        public static extern int lua_pcallk(IntPtr L, int nargs, int nresults, int msgh, IntPtr ctx, IntPtr k);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushnil")]
        public static extern void lua_pushnil(IntPtr L);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushnumber")]
        public static extern void lua_pushnumber(IntPtr L, double n);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushstring")]
        public static extern void lua_pushstring(IntPtr L, string s);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushvalue")]
        public static extern void lua_pushvalue(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushboolean")]
        public static extern void lua_pushboolean(IntPtr L, int b);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushinteger")]
        public static extern void lua_pushinteger(IntPtr L, int n);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushcclosure")]
        public static extern void lua_pushcclosure(IntPtr L, IntPtr fn, int n);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_next")]
        public static extern int lua_next(IntPtr L, int n);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_settop")]
        public static extern void lua_settop(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_tointegerx")]
        public static extern int lua_tointegerx(IntPtr L, int index, IntPtr isnum);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_tonumberx")]
        public static extern double lua_tonumberx(IntPtr L, int index, IntPtr isnum);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_topointer")]
        public static extern IntPtr lua_topointer(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_toboolean")]
        public static extern int lua_toboolean(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_tocfunction")]
        public static extern IntPtr lua_tocfunction(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_tolstring")]
        public static extern IntPtr lua_tolstring(IntPtr L, int index, IntPtr len);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_close")]
        public static extern void lua_close(IntPtr L);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getglobal")]
        public static extern int lua_getglobal(IntPtr L, string name);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setglobal")]
        public static extern void lua_setglobal(IntPtr L, string name);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setfield")]
        public static extern void lua_setfield(IntPtr L, int index, string k);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getfield")]
        public static extern int lua_getfield(IntPtr L, int index, string k);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_createtable")]
        public static extern void lua_createtable(IntPtr L, int narr, int nrec);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_setfuncs")]
        public static extern void luaL_setfuncs(IntPtr L, IntPtr l, int nup);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isboolean")]
        public static extern int lua_isboolean(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_iscfunction")]
        public static extern int lua_iscfunction(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isfunction")]
        public static extern int lua_isfunction(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isinteger")]
        public static extern int lua_isinteger(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isnil")]
        public static extern int lua_isnil(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_type")]
        public static extern int lua_type(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isnumber")]
        public static extern int lua_isnumber(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isstring")]
        public static extern int lua_isstring(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_istable")]
        public static extern int lua_istable(IntPtr L, int index);

        [DllImport("lua.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gettop")]
        public static extern int lua_gettop(IntPtr L);


        public static void lua_pushcfunction(IntPtr L, lua_CFunction fn) => lua_pushcclosure(L, Marshal.GetFunctionPointerForDelegate(fn), 0);

        public static double lua_tonumber(IntPtr L, int index) => lua_tonumberx(L, index, IntPtr.Zero);

        public static int lua_tointeger(IntPtr L, int index) => lua_tointegerx(L, index, IntPtr.Zero);

        public static IntPtr lua_tostring(IntPtr L, int index) => lua_tolstring(L, index, IntPtr.Zero);


        public static bool lua_isnone(IntPtr L, int index) => lua_type(L, index) == -1;


        public static void lua_pop(IntPtr L, int n) => lua_settop(L, -(n) - 1);

        public static int luaL_loadfile(IntPtr L, string filename) => luaL_loadfilex(L, filename, null);

        public static int lua_pcall(IntPtr L, int nargs, int nresults, int msgh)
        {
            int ret = lua_pcallk(L, nargs, nresults, msgh, IntPtr.Zero, IntPtr.Zero);
            if (ret != 0)
                Console.WriteLine(Marshal.PtrToStringAnsi(lua_tolstring(L, -1, IntPtr.Zero)));

            return ret;
        }

        public static void lua_newtable(IntPtr L) => lua_createtable(L, 0, 0);

        public static void luaL_newlibtable(IntPtr L, luaL_Reg[] l) => lua_createtable(L, 0, l.Length);

        public static void luaL_newlib(IntPtr L, luaL_Reg[] l)
        {
            luaL_newlibtable(L, l);

            int elementSize = Marshal.SizeOf(typeof(luaL_Reg));
            IntPtr lPtr = Marshal.AllocHGlobal(l.Length * elementSize);
            for (int i = 0; i < l.Length; i++)
                Marshal.StructureToPtr(l[i], new IntPtr((long)lPtr + i * elementSize), false);

            luaL_setfuncs(L, lPtr, 0);
        }
    }
}
