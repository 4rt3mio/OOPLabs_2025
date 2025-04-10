using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OOPsl.DocumentFunctions.Converters
{
    public class XmlToJsonConverter : IFormatConverter
    {
        public string Convert(string input)
        {
            try
            {
                var doc = XDocument.Parse(input);
                string json = JsonConvert.SerializeXNode(doc, Formatting.Indented, omitRootObject: true);
                return json;
            }
            catch (Exception ex)
            {
                return $"Ошибка конвертации XML → JSON: {ex.Message}";
            }
        }
    }
}
