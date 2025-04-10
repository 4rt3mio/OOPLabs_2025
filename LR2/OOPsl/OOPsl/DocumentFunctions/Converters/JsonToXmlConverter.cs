using Newtonsoft.Json;
using System.Xml.Linq;

namespace OOPsl.DocumentFunctions.Converters
{
    public class JsonToXmlConverter : IFormatConverter
    {
        public string Convert(string input)
        {
            try
            {
                var xDoc = JsonConvert.DeserializeXNode(input, "root");
                return xDoc.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка конвертации JSON → XML: {ex.Message}";
            }
        }
    }
}
