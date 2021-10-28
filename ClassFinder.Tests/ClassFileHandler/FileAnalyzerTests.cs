using ClassFinder.ClassFileHandler;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassFinder.Tests.ClassFileHandler
{
    class FileAnalyzerTests
    {
        FileAnalyzer fileAnalyzer;
        List<string> content;

        [SetUp]
        public void Setup()
        {
            fileAnalyzer = new FileAnalyzer();
            content = new List<string>
            {
                "a.b.FooBarBaz",
                "c.d.FooBar",
                "codeborne.WishMaker",
                "codeborne.MindReader",
                "TelephoneOperator",
                "ScubaArgentineOperator",
                "YoureLeavingUsHere",
                "YouveComeToThisPoint",
                "YourEyesAreSpinningInTheirSockets"
            };
        }

        [TestCase("FooBar", 2)]
        [TestCase("FooBar ", 1)]
        [TestCase("fBb", 0)]
        [TestCase("EyesSp*n", 1)]
        [TestCase("TO", 1)]
        public void GetMatches_ForUpperCaseSearch(string searchParam, int count)
        {
            var answers = fileAnalyzer.GetValidNamesWithCapitalLetters(searchParam, content);

            Assert.IsTrue(answers.Count == count);
        }

        [TestCase("fbb", 1)]
        [TestCase("fb", 2)]
        [TestCase("cw", 1)]
        [TestCase("to", 1)]
        [TestCase("bf", 0)]
        public void GetMatches_ForLowerCaseSearch(string searchParam, int count)
        {
            var answers = fileAnalyzer.GetValidNamesCaseInsensitive(searchParam, content);

            Assert.IsTrue(answers.Count == count);
        }

        [Test]
        public void GetSearchParamsFromUpperCaseString()
        {
            var searchParam = "LeaveUHere";

            var paramsList = fileAnalyzer.GetSearchParamsByUpperCase(searchParam);

            Assert.That(paramsList.Count == 3);
            Assert.That(paramsList.Last() == "Here");
        }

        [TestCase(0, "Leave")]
        [TestCase(5, "U")]
        [TestCase(6, "Here")]
        public void GetIndex(int startIndex, string expectedResult)
        {
            var searchParam = "LeaveUHere";
            var indexList = new List<int>()
            {
                0, 5,6
            };

            var result = fileAnalyzer.GetParameterFromParametersString(searchParam, indexList, startIndex);

            Assert.That(result == expectedResult);
        }

        [TestCase("FoBar", true)]
        [TestCase("FoZarBarZar", true)]
        [TestCase("FooBarZar", true)]
        [TestCase("FoBa", false)]
        [TestCase("BarFo", false)]
        [TestCase("BarFooZar", false)]
        [TestCase("fooBarzar", false)]
        [TestCase("LoremIpsum", false)]
        public void LineComparison_IsTrue_WhenMatchesSearch(string line, bool expectedResult)
        {
            var searchParam = new List<string> { "Fo", "Bar " };

            var result = fileAnalyzer.GetLineCorrespondToSearchParameters(line, searchParam);

            Assert.IsTrue(result == expectedResult);
        }

        [TestCase("Bar", true)]
        [TestCase("FooBar", true)]
        [TestCase("FooBarzar", true)]
        [TestCase("Ba", false)]
        [TestCase("FooBarZar", false)]
        public void LineComparison_IsTrue_WhenRequiredFinalParameterMatches(string line, bool expectedResult)
        {
            var searchParam = "Bar ";

            var result = fileAnalyzer.GetIsRequiredFinalParameterMatch(line, searchParam);

            Assert.IsTrue(result == expectedResult);
        }

        [TestCase("Spin", true)]
        [TestCase("S*in", true)]
        [TestCase("S**n", true)]
        [TestCase("S**", true)]
        [TestCase("Saan", false)]
        [TestCase("S*n", false)]
        [TestCase("Spinning*", false)]
        public void WildcardComparisonFindsMatch(string parameter, bool expectedResult)
        {
            var line = "Spinning";

            var result = fileAnalyzer.GetIsMatchWithWildcard(line, parameter);

            Assert.IsTrue(result == expectedResult);
        }

        [TestCase("EyesSpinning")]
        [TestCase("SpSpinning")]
        public void ComplexWildcardComparisonFindsMatch(string line)
        {
            var parameter = "Sp*n";

            var result = fileAnalyzer.GetIsMatchWithWildcard(line, parameter);

            Assert.IsTrue(result);
        }
    }
}
