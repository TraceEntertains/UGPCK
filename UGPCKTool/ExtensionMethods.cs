using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGPCKTool
{
    public static class ExtensionMethods
    {
        public static byte[] FromBase64Bytes(this byte[] base64Bytes)
        {
            string base64String = Encoding.UTF8.GetString(base64Bytes, 0, base64Bytes.Length);
            return Convert.FromBase64String(base64String);
        }

        public static byte[] ToBase64Bytes(this byte[] base64Bytes)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(base64Bytes));
        }
    }
}
