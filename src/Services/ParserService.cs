using System;
using System.Collections.Generic;
using System.Linq;

using LispParser.Contracts;

namespace LispParser.Services
{
    public class ParserService : IParserService
    {
        const char LeftParenthesis = '(';
        const char RightParenthesis = ')';
        const char Quote = '"';

        public string RemoveComments(string content)
        {
            var rows = content
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries)
                .ToList();

            rows.RemoveAll(row => row.StartsWith(";"));
            var parsedRows = new List<string>();

            foreach (var row in rows)
            {
                parsedRows.Add(RemoveCommentFromRow(row));
            }

            var parsedContents = string.Join(string.Empty, parsedRows);
            return parsedContents;
        }

        public string RemoveCommentFromRow(string row)
        {
            var index = row.IndexOf(";");
            var newRow = row;
            if (index >= default(int))
            {
                newRow = row.Substring(0, index);
            }
            return newRow;
        }

        public string RemoveQuotes(string content)
        {
            var index = 0;
            index = content.IndexOf(Quote);

            while (index >= 0)
            {
                content = RemoveNextQuoteSet(content, index);
                index = content.IndexOf(Quote);
            }

            return content;
        }

        public string RemoveNextQuoteSet(string content, int index)
        {
            var secondQuoteIndex = content.IndexOf(Quote, index + 1);

            if (secondQuoteIndex < 0)
            {
                throw new ArgumentException("Number of quotes is uneven; file cannot be parsed.");
            }

            return content.Substring(0, index) + content.Substring(secondQuoteIndex + 1);
        }

        public void ValidateCount(string content)
        {
            var leftParenthesisCount = content.Count(s => s == LeftParenthesis);
            var rightParenthesisCount = content.Count(s => s == RightParenthesis);

            if (leftParenthesisCount != rightParenthesisCount)
            {
                throw new ArgumentException($"Found {leftParenthesisCount} '(' and {rightParenthesisCount} ')'.");
            }
        }

        public void ValidateNesting(string content)
        {
            int leftCount = 0;
            foreach (char character in content)
            {
                leftCount = ParseForParentheses(character, leftCount);
            }
        }

        public int ParseForParentheses(char character, int leftCount)
        {
            switch (character)
            {
                case LeftParenthesis:
                    leftCount++;
                    break;
                case RightParenthesis:
                    leftCount = HandleRightParentheses(leftCount);
                    break;
                default:
                    break;
            }
            return leftCount;
        }

        public int HandleRightParentheses(int leftCount)
        {
            if (leftCount == 0)
            {
                throw new ArgumentException("Improperly nested ')'.");
            }
            return leftCount - 1;
        }
    }
}
