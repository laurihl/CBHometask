using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ClassFinder.ClassFileHandler
{
    public class FileReader
    {
        public string NewInput;

        public List<string> GetFileContents(List<string> originalInput)
        {
            var lines = WaitForCorrectFileName(originalInput);
            return lines;
        }

        private List<string> WaitForCorrectFileName(List<string> originalInput)
        {
            var fileContent = new List<string>();
            if (originalInput.Count > 0)
            {
                fileContent = GetFileContent(originalInput.First());
            }

            while (!fileContent.Any())
            {
                Console.WriteLine("Please input correct file name and parameter (without brackets) or type 'exit' to exit the app:");
                var newInput = Console.ReadLine();

                if (newInput == "exit")
                {
                    return fileContent;
                }
                else if (!string.IsNullOrWhiteSpace(newInput))
                {
                    NewInput = newInput.Substring(newInput.IndexOf(" ") +1);
                    fileContent = GetFileContent(newInput.Split(" ").First());
                }
            }

            return fileContent;
        }

        private List<string> GetFileContent(string input)
        {
            var directory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var filename = input.EndsWith(".txt") ? input : input + ".txt";
            var filePath = Path.Combine(directory, filename);
            return ReadFile(filePath);
        }

        public List<string> ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Failed to find file: {path}");
                return new List<string>();
            }
            return File.ReadLines(path).ToList();
        }
    }
}
