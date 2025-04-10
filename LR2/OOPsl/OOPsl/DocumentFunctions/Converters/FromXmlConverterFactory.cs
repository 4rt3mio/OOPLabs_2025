namespace OOPsl.DocumentFunctions.Converters
{
    public static class FromXmlConverterFactory
    {
        public static IFromXmlConverter GetConverter(string targetExtension)
        {
            switch (targetExtension.ToLower())
            {
                case "txt":
                    return new XmlToTxtConverter();
                case "md":
                    return new XmlToMdConverter();
                case "json":
                    return new XmlToJsonConverter();
                case "xml":
                    return new XmlIdentityConverter();
                case "rtf":
                    return new XmlToRtfConverter();
                default:
                    throw new NotSupportedException($"Целевой формат \"{targetExtension}\" не поддерживается.");
            }
        }
    }
}
