using System.Text.RegularExpressions;

namespace OOPsl.DocumentFunctions.Converters
{
    public class RtfToMarkdownConverter : IFormatConverter
    {
        public string Convert(string input)
        {
            string md = input;

            md = ProcessNestedTags(md, 3); 
            md = ProcessNestedTags(md, 2); 
            md = ProcessSingleTags(md);    

            return md;
        }

        private string ProcessNestedTags(string input, int depth)
        {
            string pattern = $@"(<([ubi])>){RepeatPattern("<([ubi])>", depth - 1)}(.+?){RepeatPattern("</\\3>", depth - 1)}</\\2>";
            return Regex.Replace(input, pattern, match => ConvertTagGroup(match, depth), RegexOptions.Singleline);
        }

        private string ProcessSingleTags(string input)
        {
            return Regex.Replace(input, @"<([ubi])>(.+?)</\1>", match =>
                ConvertTagGroup(match, 1),
                RegexOptions.Singleline);
        }

        private string RepeatPattern(string pattern, int count)
        {
            return string.Join("", Enumerable.Repeat(pattern, count));
        }

        private string ConvertTagGroup(Match match, int depth)
        {
            var tags = new List<string>();
            var content = match.Groups[3 + (depth - 1) * 2].Value;

            for (int i = 0; i < depth; i++)
            {
                tags.Add(match.Groups[2 + i * 2].Value);
            }

            var orderedTags = tags
                .OrderBy(t => t == "u" ? 0 : t == "b" ? 1 : 2)
                .ToList();

            foreach (var tag in orderedTags)
            {
                content = ApplyTag(content, tag);
            }

            return content;
        }

        private string ApplyTag(string content, string tag)
        {
            return tag switch
            {
                "u" => $"<u>{content}</u>",
                "b" => $"**{content}**",
                "i" => $"*{content}*",
                _ => content
            };
        }
    }
}