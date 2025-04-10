namespace OOPsl.DocumentFunctions.Converters
{
    public class XmlIdentityConverter : IToXmlConverter, IFromXmlConverter
    {
        public string ConvertToXml(string content) => content;
        public string ConvertFromXml(string xml) => xml;
    }
}
