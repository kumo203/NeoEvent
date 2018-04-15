using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace NeoWriteReadFunctionContract
{
    public class WriteReadFunctionContract : SmartContract // FunctionCode
    {
        public static object Main(string operation, params object[] args)
        {
            Runtime.Notify("Logging Initialize:");

            object result = "none";
            if (operation == "Read")
            {
                string arg0 = (string)args[0];
                Runtime.Notify("Logging Read:", arg0);
                result = Read(arg0);
            }

            if (operation == "Write")
            {
                string arg0 = (string)args[0];
                string arg1 = (string)args[1];
                Runtime.Notify("Logging Write:", arg0, arg1);
                result = Write(arg0, arg1);
            }

            Runtime.Notify("Logging Finalize:", result);
            return result;
        }

        public static bool Write(string key, string value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
            return true;
        }

        public static byte[] Read(string key)
        {
            return Storage.Get(Storage.CurrentContext, key);
        }
    }
}
