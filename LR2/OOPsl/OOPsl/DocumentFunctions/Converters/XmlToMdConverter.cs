namespace OOPsl.DocumentFunctions.Converters
{
    public class XmlToMdConverter : IFromXmlConverter
    {
        public string ConvertFromXml(string xml)
        {
            string content = xml
                .Replace("<document>", "")
                .Replace("</document>", "")
                .Replace("<strong>", "**")
                .Replace("<em>", "*");
            return content;
        }
    }
}