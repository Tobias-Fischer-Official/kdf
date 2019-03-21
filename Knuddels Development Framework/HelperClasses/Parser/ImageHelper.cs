using System;
using System.Collections.Generic;
using System.Drawing;
using KDF.HelperClasses.Parser.ParserObjects;
using System.Xml.Serialization;
using System.IO;

namespace KDF.HelperClasses.Parser
{
    public class ImageHelper
    {
        public static Image LoadImage(string filename)
        {
            return null;
        }

        public static Image LoadImage(Uri imageaddress)
        {
            return null;
        }

        public static string DecodeImageName(string imagecode)
        {
            if (imagecode.Contains("..."))
                return imagecode.Split(new string[] { "..." }, StringSplitOptions.RemoveEmptyEntries)[0] + "." + imagecode.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[imagecode.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
            else
                return imagecode;
        }

        public static void SerializeImage(byte[] data, List<string> format, string name)
        {
            string imageName = name.Substring(0, name.LastIndexOf("."));
            string imageExtension = name.Split('.')[name.Split('.').Length-1];

            XMLImage xmlImage = new XMLImage();
            xmlImage.Data = data;
            xmlImage.Extension = imageExtension;
            xmlImage.Format = format;

            XmlSerializer ser = new XmlSerializer(typeof(XMLImage));
            StreamWriter writer = new StreamWriter("icon.xml");
            ser.Serialize(writer, xmlImage);
            writer.Flush();
            writer.Close();
        }

        public XMLImage DeserializeImage(string imageName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XMLImage));
            StreamReader reader = new StreamReader("icon.xml");
            XMLImage image = (XMLImage)ser.Deserialize(reader);
            reader.Close();

            return image;
        }
    }
}
