using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OOPsl.DocumentFunctions.Displays
{
    public class MarkdownDisplayStrategy : IDisplayStrategy
    {
        private const string ResetAnsi = "\x1b[0m";
        private readonly Stack<string> _styleStack = new Stack<string>();
        private readonly Stack<string> _tagStack = new Stack<string>();
        private readonly List<string> _errors = new List<string>();

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
            Console.WriteLine(content + ResetAnsi);
        }

        private List<string> ValidateMarkdown(string content)
        {
            var errors = new List<string>();
            var stack = new Stack<string>();
            var matches = Regex.Matches(content, @"(\*{3}|\*{2}|\*|<u>|<\/u>)");

            foreach (Match match in matches)
            {
                string token = match.Value;
                int position = match.Index;

                switch (token)
                {
                    case "<u>":
                        stack.Push("u");
                        break;
                    case "</u>":
                        if (stack.Count == 0 || stack.Pop() != "u")
                            errors.Add($"Непарный тег </u> на позиции {position}");
                        break;
                    default:
                        HandleAsterisks(token, position, stack, errors);
                        break;
                }
            }

            while (stack.Count > 0)
                errors.Add($"Незакрытый тег: {stack.Pop()}");

            return errors;
        }

        private void HandleAsterisks(string token, int position, Stack<string> stack, List<string> errors)
        {
            string tagType = token switch
            {
                "*" => "i",
                "**" => "b",
                "***" => "bi",
                _ => throw new ArgumentException("Недопустимый тег")
            };

            if (stack.Count > 0 && stack.Peek() == tagType)
            {
                stack.Pop();
            }
            else
            {
                stack.Push(tagType);
            }
        }

        private void ApplyFormatting(ref string content)
        {
            content = Regex.Replace(content, @"<u>\*\*\*(.+?)\*\*\*</u>", 
                m => $"\x1b[4;1;3m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*\*\*(.+?)\*\*\*", 
                m => $"\x1b[1;3m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"<u>(.+?)</u>", 
                m => $"\x1b[4m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*\*(.+?)\*\*", 
                m => $"\x1b[1m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*(?!\*)(.+?)\*", 
                m => $"\x1b[3m{m.Groups[1].Value}\x1b[0m", RegexOptions.Singleline);
        }
    }
}