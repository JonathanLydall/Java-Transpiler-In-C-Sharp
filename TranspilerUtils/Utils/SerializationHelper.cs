using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TranspilerUtils.Utils
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                return default(T);
            }

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                memoryStream.Position = 0;
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(memoryStream);
            }
        }

        public static string Serialize<T>(T item)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    try
                    {
                        var xmlSerializer = new XmlSerializer(item.GetType());
                        xmlSerializer.Serialize(writer, item);
                    }
                    catch (Exception e)
                    {
                        var exception = e;
                        while (exception.InnerException != null)
                        {
                            Console.WriteLine(exception.InnerException.Message);
                            exception = exception.InnerException;
                        }
                    }

                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
    }
}
