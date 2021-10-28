using ClassFinder.ClassFileHandler;
using System;
using System.Linq;

namespace ClassFinder
{
    public class Program
    {
        static void Main(string[] args)
        {
            var input = args.ToList();
            var fileReader = new FileReader();
            var content = fileReader.GetFileContents(input).Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (!content.Any())
            {
                Console.WriteLine("No content input. Exiting ...");
                return;
            }

            var parameters = input.Count > 1 ? input[1] : fileReader.NewInput;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("No parameters found.");
                return;
            }

            var searchResults = new FileAnalyzer().GetValidClassNames(parameters, content);

            foreach (var result in searchResults.OrderBy(c=>c.Trim()).ThenBy(c => !c.Contains('.')))
            {
                Console.WriteLine(result.Trim());
            }

            if (!searchResults.Any())
                Console.WriteLine("No results found.");

            Console.WriteLine("Exiting ...");
        }
    }
}
