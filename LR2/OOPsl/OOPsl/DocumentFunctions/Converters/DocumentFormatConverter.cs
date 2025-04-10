namespace OOPsl.DocumentFunctions.Converters
{
    public class DocumentFormatConverter
    {
        public string Convert(string inputContent, string sourceExtension, string targetExtension)
        {
            if (sourceExtension.Equals(targetExtension, StringComparison.OrdinalIgnoreCase))
            {
                return inputContent;
            }

            IToXmlConverter toXmlConverter = ToXmlConverterFactory.GetConverter(sourceExtension);
            string xml = toXmlConverter.ConvertToXml(inputContent);

            IFromXmlConverter fromXmlConverter = FromXmlConverterFactory.GetConverter(targetExtension);
            string result = fromXmlConverter.ConvertFromXml(xml);

            return result;
        }
    }
}
