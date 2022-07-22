using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGPCKTool
{
    public static class UGPCKExtractor
    {
        public static UGPCK? UGPCKExtract(string path)
        {
            string? dirPath = Path.GetDirectoryName(Path.GetFullPath(path));

            Directory.CreateDirectory(dirPath + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(path));

            byte[] pckBytes = File.ReadAllBytes(dirPath + Path.DirectorySeparatorChar + Path.GetFileName(path));

            MemoryStream memStream = new(pckBytes);
            StreamWriter streamWriter = new(memStream);
            streamWriter.Write(Encoding.UTF8.GetString(pckBytes));
            memStream.Flush();
            memStream.Position = 0;

            BinaryReader binaryReader = new(memStream);

            bool FileEnded = false;
            string previousType = "StartFileMagic";
            bool lastReadSeparatorChar = false;

            if (Encoding.UTF8.GetString(binaryReader.ReadBytes(UGPCK.MagicString.Length)) == UGPCK.MagicString)
            {
                Console.WriteLine("Attempting to extract...");
            }
            else
            {
                throw new Exception("Not a valid UGPCK file.");
            }

            UGPCK UGPCKFile = new();
            PackFile pckFile = new();

            while (!FileEnded)
            {
                if (!lastReadSeparatorChar)
                {
                    ReadPCKBorder(ref binaryReader, Program.PCKSeparatorChar, previousType, ref lastReadSeparatorChar);
                    lastReadSeparatorChar = true;
                }

                switch (previousType)
                {
                    case "FileStream":
                    case "StartFileMagic":
                        previousType = "StartEmbeddedFileHeader";
                        lastReadSeparatorChar = false;
                        previousType = ReadPCKBorder(ref binaryReader, PackFile.MagicPack, previousType, ref lastReadSeparatorChar);
                        break;
                    case "StartEmbeddedFileHeader":
                        pckFile.NameSize = ReadUntilString(ref binaryReader);
                        previousType = "NameBytesLength";
                        lastReadSeparatorChar = false;
                        break;
                    case "NameBytesLength":
                        pckFile.Name = Encoding.UTF8.GetString(binaryReader.ReadBytes(pckFile.NameSize));
                        Console.WriteLine($"{pckFile.Name} | {pckFile.NameSize} bytes long");
                        previousType = "FileName";
                        lastReadSeparatorChar = false;
                        break;
                    case "FileName":
                        pckFile.FileStreamSize = ReadUntilString(ref binaryReader);
                        previousType = "FileStreamBytesLength";
                        lastReadSeparatorChar = false;
                        break;
                    case "FileStreamBytesLength":
                        byte[] decodedData = Convert.FromBase64String(Encoding.UTF8.GetString(binaryReader.ReadBytes(pckFile.FileStreamSize)));
                        MemoryStream fileStream = new(Encoding.UTF8.GetString(decodedData).Length);
                        StreamWriter fileStreamWriter = new(fileStream);
                        fileStreamWriter.Write(Encoding.UTF8.GetString(decodedData));
                        fileStreamWriter.Flush();
                        Console.WriteLine(Encoding.UTF8.GetString(decodedData));
                        pckFile.FileStream = fileStream;
                        UGPCKFile.Files.Add(pckFile);
                        pckFile = new();
                        previousType = "FileStream";
                        lastReadSeparatorChar = false;
                        break;
                    case "FileEnd":
                        FileEnded = true;
                        return UGPCKFile;
                    default:
                        Environment.Exit(-1);
                        break;
                }
            }

            return null;
        }

        public static string ReadPCKBorder(ref BinaryReader binRead, string header, string previousType, ref bool lastReadSeparatorCharacter)
        {
            string headerToMatch = Encoding.UTF8.GetString(binRead.ReadBytes(header.Length));
            if (header == PackFile.MagicPack && headerToMatch == Program.PCKEndChar)
            {
                Console.WriteLine($"{headerToMatch} | {Program.PCKEndChar}");
                Console.WriteLine("File Ended");
                lastReadSeparatorCharacter = true;
                return "FileEnd";
            }

            Console.WriteLine($"{headerToMatch} | {header}");

            if (headerToMatch == header)
            {
                Console.WriteLine("Read header/separator");
                return previousType;
            }
            else
            {
                Console.WriteLine(previousType);
                throw new Exception("Not a valid UGPCK file.");
            }
        }

        public static int ReadUntilString(ref BinaryReader binRead)
        {
            byte[] readByte = new byte[1];
            char[] readChar = new char[1];
            int binReturn = 0;

            Type? binReadType = null;

            while (binReadType != typeof(string))
            {
                if (int.TryParse(((char)binRead.PeekChar()).ToString(), out _))
                {
                    readByte[0] = binRead.ReadByte();
                    Encoding.UTF8.GetChars(readByte, readChar);
                    var binReturnString = binReturn.ToString();
                    binReturnString += readChar[0].ToString();
                    binReturn = int.Parse(binReturnString);
                }
                else
                {
                    binReadType = typeof(string);
                }
            }

            return binReturn;
        }

        public static void UGPCKCreateFiles(UGPCK UGPCK)
        {
            foreach (PackFile file in UGPCK.Files)
            {
                if (!Directory.Exists(Path.GetDirectoryName(Environment.CurrentDirectory + $"\\{file.Name}")))
                {
                    Console.WriteLine(file.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(file.Name));
                }

                FileStream fileStream = File.Create(Environment.CurrentDirectory + "\\" + file.Name);
                file.FileStream.CopyTo(fileStream);
                fileStream.Close();
            }
        }
    }
}
