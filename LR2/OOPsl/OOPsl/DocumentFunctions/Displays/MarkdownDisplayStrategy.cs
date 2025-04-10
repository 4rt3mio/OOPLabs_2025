using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OOPsl.DocumentFunctions.Displays
{
    public class MarkdownDisplayStrategy : IDisplayStrategy
    {
        public void Display(Document document)
        {
            string content = document.Content;
            var errors = ValidateMarkdown(content);

            if (errors.Count > 0)
            {
                Console.WriteLine("Ошибки форматирования:");
                errors.ForEach(e => Console.WriteLine($"• {e}"));
                Console.WriteLine("Контент не отображён.");
                return;
            }

            ApplyFormatting(ref content);
            Console.WriteLine(content);
        }

        private List<string> ValidateMarkdown(string content)
        {
            var errors = new List<string>();
            var stack = new Stack<string>();
            var matches = Regex.Matches(content, @"(\*{1,3}|<u>|<\/u>)");

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                string token = match.Value;

                switch (token)
                {
                    case "<u>":
                        stack.Push("u");
                        break;
                    case "</u>":
                        if (stack.Count == 0 || stack.Pop() != "u")
                            errors.Add($"Непарный тег </u> на позиции {match.Index}");
                        break;
                    default:
                        HandleAsterisks(token, match.Index, stack, errors);
                        break;
                }
            }

            while (stack.Count > 0)
                errors.Add($"Незакрытый тег: {stack.Pop()}");

            return errors;
        }

        private void HandleAsterisks(string token, int position, Stack<string> stack, List<string> errors)
        {
            int count = token.Length;
            string expected = count switch { 1 => "*", 2 => "**", 3 => "***", _ => "" };

            if (stack.Count > 0 && stack.Peek() == expected)
                stack.Pop();
            else
                stack.Push(expected);
        }

        private void ApplyFormatting(ref string content)
        {
            content = Regex.Replace(content, @"<u>\*\*\*(.+?)\*\*\*</u>",
                m => $"\x1b[4;1;3m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"<u>(.+?)</u>",
                m => $"\x1b[4m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*\*\*(.+?)\*\*\*",
                m => $"\x1b[1;3m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*\*(.+?)\*\*",
                m => $"\x1b[1m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*(.+?)\*",
                m => $"\x1b[3m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);
        }
    }
}