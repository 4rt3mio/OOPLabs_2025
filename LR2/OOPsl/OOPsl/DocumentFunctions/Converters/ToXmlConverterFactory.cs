namespace OOPsl.DocumentFunctions.Converters
{
    public static class ToXmlConverterFactory
    {
        public static IToXmlConverter GetConverter(string sourceExtension)
        {
            switch (sourceExtension.ToLower())
            {
                case "txt":
                    return new TxtToXmlConverter();
                case "md":
                    return new MdToXmlConverter();
                case "json":
                    return new JsonToXmlConverter();
                case "xml":
                    return new XmlIdentityConverter();
                case "rtf":
                    return new RtfToXmlConverter();
                default:
                    throw new NotSupportedException($"Исходный формат \"{sourceExtension}\" не поддерживается.");
            }
        }
    }
}
