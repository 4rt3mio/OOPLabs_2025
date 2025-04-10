using System.Text.RegularExpressions;

namespace OOPsl.DocumentFunctions.Displays
{
    public class MarkdownDisplayStrategy : IDisplayStrategy
    {
        public void Display(Document document)
        {
            string content = document.Content;

            content = Regex.Replace(content, @"<u>\*\*\*(.+?)\*\*\*</u>", match =>
            {
                return "\x1b[4m\x1b[1m\x1b[3m" + match.Groups[1].Value + "\x1b[0m";
            }, RegexOptions.Singleline);

            content = Regex.Replace(content, @"<u>(.+?)</u>", match =>
            {
                return "\x1b[4m" + match.Groups[1].Value + "\x1b[0m";
            }, RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*\*\*(.+?)\*\*\*", match =>
            {
                return "\x1b[1m\x1b[3m" + match.Groups[1].Value + "\x1b[0m";
            }, RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*\*(.+?)\*\*", match =>
            {
                return "\x1b[1m" + match.Groups[1].Value + "\x1b[0m";
            }, RegexOptions.Singleline);

            content = Regex.Replace(content, @"\*(.+?)\*", match =>
            {
                return "\x1b[3m" + match.Groups[1].Value + "\x1b[0m";
            }, RegexOptions.Singleline);

            Console.WriteLine(content);
        }
    }
}
