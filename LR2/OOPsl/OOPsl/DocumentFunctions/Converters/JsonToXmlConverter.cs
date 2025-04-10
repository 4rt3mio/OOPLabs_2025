namespace OOPsl.DocumentFunctions.Converters
{
    class JsonToXmlConverter : IToXmlConverter
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