using System;
using System.Collections.Generic;
using System.IO;

namespace Root
{
    public static class ProgramUtil
    {
        public static void ValidateArgs(string[] args, string inputPath)
        {                
            if (args.Length != 1)
                throw new ApplicationException("Usage -> arg[0]: <inputFileName>");
            
                string inputFilePath = Path.Combine(inputPath, args[0]);
            if (!File.Exists(inputFilePath))
                throw new ApplicationException($"Input file provided {inputFilePath} does not exist");
        }
        public static List<string> GetRawInput(string path)
        {
            var rawInput = new List<string>();
            using (var sr = new StreamReader(path))
            {
                string lineIn;
                while((lineIn = sr.ReadLine()) != null)
                {
                    if (lineIn.Trim() != "")
                        rawInput.Add(lineIn);
                }                
                return rawInput;
            }
        }       
        public static void ConsoleColorWriteLine(string message, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }        
    }
}
