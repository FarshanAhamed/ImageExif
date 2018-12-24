using System;
using System.IO;

namespace ImageExif
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles($"{Directory.GetCurrentDirectory()}/Images", "*.JPG");
            foreach (string file in files)
            {
                try
                {

                    Console.Write(Path.GetFileName(file) + " => ");
                    var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
                    var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    var output = ImageFunction.FixExif(ms).GetAwaiter().GetResult();
                    if (output == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("FAIL : Exif Function returned null");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SUCCESS : Exif Function returned stream");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"EXCEPTION : {ex.Message}");
                    Console.ResetColor();
                }
            }


            Console.ReadLine();
        }
    }
}
