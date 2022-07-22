using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGPCKTool
{
    public class UGPCK
    {
        public static readonly string MagicString = "UGPCK";
        public List<PackFile> Files = new();

        public static byte[] GetBytes(UGPCK UGPCK)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(MagicString);
            bytes = bytes
                .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar)
                .Concat(PackFile.GetBytes(UGPCK)).ToArray()
                .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar)
                .Concat(Encoding.UTF8.GetBytes(Program.PCKEndChar)))).ToArray();
            return bytes;
        }
    }

    public class PackFile
    {
        public static readonly string MagicPack = "PCKSEC";
        public int NameSize;
        public string Name;
        public int FileStreamSize;
        public MemoryStream FileStream;

        public static byte[] GetBytes(UGPCK UGPCK)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(MagicPack);

            foreach (PackFile packFile in UGPCK.Files)
            {
                if (packFile.Name == UGPCK.Files[0].Name)
                {
                    byte[] byteData = packFile.FileStream.ToArray();

                    bytes = bytes
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(packFile.NameSize.ToString()).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(packFile.Name).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(packFile.FileStreamSize.ToString()).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(byteData)))))))))).ToArray();

                    Console.WriteLine(Encoding.UTF8.GetBytes(Convert.ToBase64String(byteData)));
                }
                else
                {
                    byte[] byteData = packFile.FileStream.ToArray();

                    bytes = bytes
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(MagicPack)).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(packFile.NameSize.ToString()).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(packFile.Name).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(packFile.FileStreamSize.ToString()).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Program.PCKSeparatorChar).ToArray()
                        .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(byteData))))))))))).ToArray();
                }
            }

            return bytes;
        }
    }
}
