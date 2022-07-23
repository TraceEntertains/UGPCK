using UGPCKTool;

public static class Program
{
    public static string[] args;
    public static string PCKSeparatorChar = "PCKBorder";
    public static string PCKEndChar = "PCKEnd";

    public static void Main(string[] args)
    {
        Program.args = args;

        if (args.Length == 0)
        {
            Console.WriteLine("no args detected, exiting program");
            Environment.Exit(-1);
        }

        switch (args[0])
        {
            case "--extract":
            case "-e":
                UGPCK? UGPCKFile = UGPCKExtractor.UGPCKExtract(args[1]);
                UGPCKExtractor.UGPCKCreateFiles(UGPCKFile, args[1]);
                break;
            case "--create":
            case "-c":
                var stream = UGPCKCreate.UGPCKFileCreate(UGPCKCreate.CreateUGPCK(args[1]));
                stream.Close();
                break;
        }
    }
}