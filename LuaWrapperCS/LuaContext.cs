using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LuaWrapper
{
    public enum LuaState : int
    {
        OK,
        YIELD,
        ERRRUN,
        ERRSYNTAX,
        ERRMEM,
        ERRGCMM,
        ERRERR,
        ERRFILE
    }

    public class LuaContext : IDisposable
    {
        private IntPtr _luaState;

        /// <summary>
        /// Creates a new instance of a LuaContext
        /// </summary>
        public LuaContext()
        {
            _luaState = NativeCalls.luaL_newstate();
            NativeCalls.luaL_openlibs(_luaState);
        }

        /// <summary>
        /// Destroys the LuaContext on disposal of object
        /// </summary>
        public void Dispose()
        {
            NativeCalls.lua_close(_luaState);
        }

        /// <summary>
        /// Loads a script from a given file
        /// </summary>
        /// <param name="filepath">File to load the script from</param>
        /// <returns>State of Load</returns>
        public LuaState LoadFromFile(string filepath)
        {
            return (LuaState)NativeCalls.luaL_loadfile(_luaState, filepath);
        }

        /// <summary>
        /// Loads a script from a given string
        /// </summary>
        /// <param name="filecontent">String to load as the script</param>
        /// <returns>State of load</returns>
        public LuaState LoadFromString(string filecontent)
        {
            return (LuaState)NativeCalls.luaL_loadstring(_luaState, filecontent);
        }

        /// <summary>
        /// Execute the script globally
        /// </summary>
        /// <returns>State of execution</returns>
        public LuaState Execute()
        {
            int ret = NativeCalls.lua_pcall(_luaState, 0, 0, 0);
            NativeCalls.lua_pop(_luaState, 1);
            return (LuaState)ret;
        }

        /// <summary>
        /// Execute specific function
        /// </summary>
        /// <param name="functionName">Name of the function</param>
        /// <param name="args">Parameters to pass to the function</param>
        /// <returns>Result of the function</returns>
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

        /// <summary>
        /// Registers any function to the context
        /// </summary>
        /// <param name="nameSpace">Namespace of the function</param>
        /// <param name="functionName">Name of the function</param>
        /// <param name="function">Function delegate</param>
        public void RegisterFunction(string nameSpace, string functionName, Delegate function)
        {
            NativeCalls.lua_pushnil(_luaState);
            NativeCalls.lua_setglobal(_luaState, (nameSpace) + "." + functionName);

            luaL_Reg[] regs = new luaL_Reg[] {
                new luaL_Reg() { name =  Marshal.StringToHGlobalAnsi(functionName), func = Marshal.GetFunctionPointerForDelegate(new lua_CFunction((IntPtr L) => {
                    var delegateParaCount=function.Method.GetParameters().Length;

                    var n = NativeCalls.lua_gettop(L);
                    if(n < delegateParaCount){
                        NativeCalls.lua_settop(L,0);
                        throw new InvalidOperationException($"Lua gave {n} parameters. {delegateParaCount} expected.");
                    }

                    List<object> args = new List<object>();
                    for(int i = 0; i < delegateParaCount; i++) {
                        var obj=LuaToObject(-1);

                        if(function.Method.GetParameters()[delegateParaCount-1-i].ParameterType.IsPrimitive)
                            obj=LuaDoubleToNumeric(obj,function.Method.GetParameters()[delegateParaCount-1-i].ParameterType);

                        args.Add(obj);
                        NativeCalls.lua_pop(_luaState, 1);
                    }

                    args.Reverse();

                    if (function.Method.ReturnType==typeof(void))
                    {
                        function.DynamicInvoke(args.ToArray());

                        return 0;
                    }
                    else if (function.Method.ReturnType.IsPrimitive || function.Method.ReturnType==typeof(string) || function.Method.ReturnType==typeof(decimal))
                    {
                        object retVal = function.DynamicInvoke(args.ToArray());

                        PushObject(retVal);

                        return 1;
                    }
                    else if(function.Method.ReturnType.Name.Contains("ValueTuple"))
                    {
                        object[] retVals=TupleToArray(function.DynamicInvoke(args.ToArray()));

                        foreach(object val in retVals)
                            PushObject(val);

                        return retVals.Length;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported ReturnType {function.Method.ReturnType.ToString()} in delegate");
                    }
                })) },
                new luaL_Reg() { name = IntPtr.Zero, func = IntPtr.Zero }
            };

            NativeCalls.luaL_newlib(_luaState, regs);
            NativeCalls.lua_setglobal(_luaState, nameSpace);
        }

        /// <summary>
        /// Converts Lua double to numeric .NET type
        /// </summary>
        /// <param name="input">Double to convert</param>
        /// <param name="toConvert">Any primitive numeric .NET type</param>
        /// <returns></returns>
        private object LuaDoubleToNumeric(object input, Type toConvert)
        {
            switch (Type.GetTypeCode(toConvert))
            {
                case TypeCode.SByte: return Convert.ToSByte(input);
                case TypeCode.Byte: return Convert.ToByte(input);
                case TypeCode.Int16: return Convert.ToInt16(input);
                case TypeCode.UInt16: return Convert.ToUInt16(input);
                case TypeCode.Int32: return Convert.ToInt32(input);
                case TypeCode.UInt32: return Convert.ToUInt32(input);
                case TypeCode.Int64: return Convert.ToInt64(input);
                case TypeCode.UInt64: return Convert.ToUInt64(input);
                case TypeCode.Single: return Convert.ToSingle(input);
                case TypeCode.Decimal: return Convert.ToDecimal(input);
                case TypeCode.String: return Convert.ToString(input);
                default: return input;
            }
        }

        /// <summary>
        /// Convert ValueTuple to array
        /// </summary>
        /// <param name="tuple">ValueTuple to convert</param>
        /// <returns>Array of values</returns>
        private object[] TupleToArray(object tuple)
        {
            if (!(tuple is ValueTuple))
                return null;

            List<object> result = new List<object>();

            foreach (var f in tuple.GetType().GetFields())
                result.Add(f.GetValue(tuple));

            return result.ToArray();
        }

        /// <summary>
        /// Converts a Lua value to a .NET object
        /// </summary>
        /// <param name="index">Index of value on the stack</param>
        /// <returns>.NET object</returns>
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

        /// <summary>
        /// Pushes value on stack
        /// </summary>
        /// <param name="arg">Value to push</param>
        private void PushObject(object arg)
        {
            if (arg is int) NativeCalls.lua_pushnumber(_luaState, Convert.ToInt32(arg));
            else if (arg is double || arg is float) NativeCalls.lua_pushnumber(_luaState, Convert.ToDouble(arg));
            else if (arg is string) NativeCalls.lua_pushstring(_luaState, (string)arg);
            else if (arg is bool) NativeCalls.lua_pushboolean(_luaState, (int)arg);
            else if (arg is null) NativeCalls.lua_pushnil(_luaState);
        }
    }
}
