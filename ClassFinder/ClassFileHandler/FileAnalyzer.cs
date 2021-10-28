using System.Collections.Generic;
using System.Linq;

namespace ClassFinder.ClassFileHandler
{
    public class FileAnalyzer
    {
        public List<string> GetValidClassNames(string searchParameter, List<string> content)
        {
            if (string.IsNullOrWhiteSpace(searchParameter))
                return new List<string>();

            var validNames = new List<string>();
            var hasUpper = searchParameter.Any(char.IsUpper);

            if (hasUpper)
            {
                validNames = GetValidNamesWithCapitalLetters(searchParameter.TrimStart(), content);
            }
            else
            {
                validNames = GetValidNamesCaseInsensitive(searchParameter.TrimStart(), content);
            }
            return validNames;
        }

        public List<string> GetValidNamesCaseInsensitive(string searchParameter, List<string> content)
        {
            var validNames = new List<string>();
            foreach (var line in content)
            {
                var lineDivisions = GetLineDivisionIndexes(line);
                var isMatch = false;
                var searchIndex = 0;
                for (int i = 0; i < lineDivisions.Count; i++)
                {
                    isMatch = char.ToLower(line[lineDivisions[i]]).Equals(searchParameter[searchIndex]);
                    if (isMatch && (lineDivisions.Count() - i < (searchParameter.Length - searchIndex)))
                    {
                        isMatch = false;
                        break;
                    }
                    else if (isMatch)
                        searchIndex++;
                    if ((searchIndex + 1) > searchParameter.Count())
                        break;
                }
                if (isMatch)
                {
                    validNames.Add(line);
                }
            }
            return validNames;
        }

        public List<string> GetValidNamesWithCapitalLetters(string searchParameter, List<string> content)
        {
            List<string> searchParams = GetSearchParamsByUpperCase(searchParameter);
            List<string> validNames = new List<string>();

            foreach (var line in content)
            {
                if (GetLineCorrespondToSearchParameters(line, searchParams) && GetIsRequiredFinalParameterMatch(line, searchParams?.Last()))
                {
                    validNames.Add(line);
                }
            }
            return validNames;
        }

        public bool GetLineCorrespondToSearchParameters(string line, List<string> searchParams)
        {
            var isMatch = false;
            foreach (var param in searchParams)
            {
                var trimmedParam = param.Trim();
                if (!GetIsMatchWithWildcard(line, trimmedParam))
                    return false;

                isMatch = true;
                var paramPositionInLine = line.IndexOf(trimmedParam);
                line = line.Length > (paramPositionInLine + 1) ? line.Substring(paramPositionInLine + trimmedParam.Length) : "";
            }
            return isMatch;
        }

        public bool GetIsRequiredFinalParameterMatch(string line, string lastParam)
        {
            if (!lastParam.EndsWith(" ")) return true;

            var indexes = GetLineDivisionIndexes(line);
            var lineEnd = line.Substring(indexes.Last());
            return GetIsMatchWithWildcard(lineEnd, lastParam.Trim());
        }

        public bool GetIsMatchWithWildcard(string line, string parameter)
        {
            var isMatch = false;

            for (int letterIndex = 0; letterIndex < line.Length; letterIndex++)
            {
                if (!parameter.StartsWith(line[letterIndex]))
                {
                    continue;
                }

                var checkLine = line.Substring(letterIndex);

                for (int i = 0; i < parameter.Length; i++)
                {
                    if (checkLine.Length < (i + 1))
                    {
                        isMatch = false;
                        break;
                    }
                    if (parameter[i].Equals('*') || checkLine[i].Equals(parameter[i]))
                    {
                        isMatch = true;
                        continue;
                    }
                    else
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    break;
            }
            return isMatch;
        }

        public List<string> GetSearchParamsByUpperCase(string searchParameter)
        {
            var paramsList = new List<string>();
            var lineDivisionIndexes = GetLineDivisionIndexes(searchParameter);

            if (lineDivisionIndexes.Any())
            {
                foreach (var letterIndex in lineDivisionIndexes)
                {
                    var param = GetParameterFromParametersString(searchParameter, lineDivisionIndexes, letterIndex);
                    paramsList.Add(param);
                }
            }

            return paramsList;
        }

        private static List<int> GetLineDivisionIndexes(string line)
        {
            var searchParametersIndexes = new List<int>() { 0 };

            for (int i = 1; i < line.Length; i++)
            {
                if (char.IsUpper(line[i]))
                {
                    searchParametersIndexes.Add(i);
                }
            }

            return searchParametersIndexes;
        }

        public string GetParameterFromParametersString(string searchParameter, List<int> divisionIndexes, int charPosition)
        {
            if (charPosition == divisionIndexes.Last())
            {
                return searchParameter.Substring(charPosition);
            }
            else
            {
                var firstCharIndex = divisionIndexes.IndexOf(charPosition);
                var takeLength = divisionIndexes[firstCharIndex + 1] - divisionIndexes[firstCharIndex];
                return searchParameter.Substring(charPosition, takeLength);
            }
        }
    }
}
