using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaWrapper
{
    public class LuaContext
    {
        private IntPtr _luaState;

        public LuaContext()
        {
            _luaState = NativeCalls.luaL_newstate();
            NativeCalls.luaL_openlibs(_luaState);
        }

        ~LuaContext()
        {
            NativeCalls.lua_close(_luaState);
        }

        public int LoadFromFile(string filepath)
        {
            return NativeCalls.luaL_loadfile(_luaState, filepath);
        }

        public int LoadFromString(string filecontent)
        {
            return NativeCalls.luaL_loadstring(_luaState, filecontent);
        }

        public int Execute()
        {
            int ret = NativeCalls.lua_pcall(_luaState, 0, 0, 0);
            NativeCalls.lua_pop(_luaState, 1);
            return ret;
        }

        public void RegisterFunction(string functionName, string @namespace, lua_CFunction function)
        {
            NativeCalls.lua_pushnil(_luaState);
            NativeCalls.lua_setglobal(_luaState, (@namespace) + "." + functionName);

            luaL_Reg[] regs = new luaL_Reg[] {
                new luaL_Reg() { name =  Marshal.StringToHGlobalAnsi(functionName), func = Marshal.GetFunctionPointerForDelegate(new lua_CFunction(function)) },
                new luaL_Reg() { name = IntPtr.Zero, func = IntPtr.Zero }
            };

            NativeCalls.luaL_newlib(_luaState, regs);
            NativeCalls.lua_setglobal(_luaState, @namespace);
        }

        public void RegisterFunction(string functionName, lua_CFunction function)
        {
            NativeCalls.lua_pushnil(_luaState);
            NativeCalls.lua_setglobal(_luaState, functionName);

            luaL_Reg[] regs = new luaL_Reg[] {
                new luaL_Reg() { name =  Marshal.StringToHGlobalAnsi(functionName), func = Marshal.GetFunctionPointerForDelegate(new lua_CFunction(function)) },
                new luaL_Reg() { name = IntPtr.Zero, func = IntPtr.Zero }
            };

            NativeCalls.luaL_newlib(_luaState, regs);
            NativeCalls.lua_setglobal(_luaState, "");
        }

        private object LuaToObject(int index)
        {
            switch (NativeCalls.lua_type(_luaState, -1))
            {
                case NativeCalls.LUA_TNIL: return null;
                case NativeCalls.LUA_TBOOLEAN: return NativeCalls.lua_toboolean(_luaState, -1) == 1;
                case NativeCalls.LUA_TNUMBER: return NativeCalls.lua_tonumber(_luaState, -1);
                case NativeCalls.LUA_TSTRING: return Marshal.PtrToStringAnsi(NativeCalls.lua_tostring(_luaState, -1));
                case NativeCalls.LUA_TFUNCTION: return Marshal.GetDelegateForFunctionPointer<lua_CFunction>(NativeCalls.lua_tocfunction(_luaState, -1));
                case NativeCalls.LUA_TTABLE:
                    List<object> table = new List<object>();

                    NativeCalls.lua_pushnil(_luaState);

                    while (NativeCalls.lua_next(_luaState, -2) != 0)
                    {
                        var ret = LuaToObject(-2);

                        table.Add(ret);

                        if (ret is object[]) continue;

                        NativeCalls.lua_pop(_luaState, 1);
                    }
                    NativeCalls.lua_pop(_luaState, 1);


                    return table.ToArray();
            }

            return null;
        }

        private void PushObject(object arg)
        {
            if (arg is int) NativeCalls.lua_pushnumber(_luaState, (int)arg);
            else if (arg is double || arg is float) NativeCalls.lua_pushnumber(_luaState, (double)arg);
            else if (arg is string) NativeCalls.lua_pushstring(_luaState, (string)arg);
            else if (arg is bool) NativeCalls.lua_pushboolean(_luaState, (int)arg);
            else if (arg is null) NativeCalls.lua_pushnil(_luaState);
        }

        public object[] Execute(string functionName, params object[] args)
        {
            NativeCalls.lua_getglobal(_luaState, functionName);

            foreach (object obj in args)
                PushObject(obj);

            NativeCalls.lua_pcall(_luaState, args.Length, -1, 0);

            List<object> retList = new List<object>();

            while (NativeCalls.lua_gettop(_luaState) >= 0)
            {
                retList.Add(LuaToObject(-2));

                NativeCalls.lua_pop(_luaState, 1);
            }

            retList.Reverse();

            return retList.ToArray();
        }

    }
}
