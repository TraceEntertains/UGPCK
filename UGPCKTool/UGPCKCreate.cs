using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UGPCKTool
{
    public static class UGPCKCreate
    {
        public static UGPCK? CreateUGPCK(string inputFolder)
        {
            if (Directory.Exists(inputFolder))
            {
                UGPCK UGPCK = new UGPCK();

                foreach (string file in Directory.EnumerateFiles(inputFolder, "*.*", SearchOption.AllDirectories))
                {
                    Console.WriteLine(file);

                    byte[] fileBytes = File.ReadAllBytes(file);

                    MemoryStream stream = new(fileBytes.ToBase64Bytes().Length);
                    stream.Write(fileBytes.ToBase64Bytes(), 0, fileBytes.ToBase64Bytes().Length);

                    var name = "";
                    
                    if (Program.args[1].EndsWith("\\"))
                    {
                        name = file.Substring(Program.args[1].Length);
                    }
                    else
                    {
                        name = file.Substring(Program.args[1].Length + 1);
                    }

                    PackFile pckfile = new()
                    {
                        NameSize = Encoding.UTF8.GetByteCount(name),
                        Name = name,
                        FileStreamSize = fileBytes.ToBase64Bytes().Length,
                        FileStream = stream
                    };

                    UGPCK.Files.Add(pckfile);
                }

                return UGPCK;
            }

            return null;
        }

        public static FileStream UGPCKFileCreate(UGPCK UGPCK)
        {
            FileStream pck = File.Create(Environment.CurrentDirectory + "\\PCKFILE.UGPCK");

            pck.Write(UGPCK.GetBytes(UGPCK));

            return pck;
        }
    }
}
