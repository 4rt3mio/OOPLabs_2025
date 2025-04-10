namespace OOPsl.DocumentFunctions.Converters
{
    class RtfToXmlConverter : IToXmlConverter
    {
        public string ConvertToXml(string content)
        {
            string xml = "<document>" + content
                .Replace("**", "<strong>")
                .Replace("*", "<em>") + "</document>";
            return xml;
        }
    }
}
