namespace OOPsl.DocumentFunctions.Converters
{
    public class MdToXmlConverter : IToXmlConverter
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
