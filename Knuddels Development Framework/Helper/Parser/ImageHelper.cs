using System;
using System.Collections.Generic;
using System.Drawing;
using KDF.Helper.Parser.ParserObjects;
using System.Xml.Serialization;
using System.IO;

namespace KDF.Helper.Parser
{
    public class ImageHelper
    {
        public static XMLImage GetImage(string fullName)
        {
            XMLImage xmlImage = new XMLImage();

            string url = DecodeImageName(fullName);
            Dictionary<string, string> imgSettings = new Dictionary<string, string>();     
            List<string> param = new List<string>();

            if (fullName.Contains("..."))
            {
                param = new List<string>(fullName.Split(new string[] { "..." }, StringSplitOptions.RemoveEmptyEntries)[1].Split('.'));

                foreach (string s in param)
                {
                    string[] keyValuePair = s.Split('_');
                    switch (keyValuePair[0])
                    {
                        case "h":
                            imgSettings.Add("max-height", keyValuePair[1] + "px");
                            break;
                        case "w":
                            imgSettings.Add("max-width", keyValuePair[1] + "px");
                            break;
                        case "quadcut":
                            imgSettings.Add("overflow", "hidden");
                            break;
                        case "border":
                            imgSettings.Add("border", keyValuePair[1]);
                            break;
                        case "shadow":
                            imgSettings.Add("box-shadow", keyValuePair[1] + "px " + keyValuePair[1] + "px white");
                            break;
                        case "mx":
                            if (keyValuePair[1].StartsWith("-"))
                                imgSettings.Add("margin-right", keyValuePair[1].Replace("-", "") + "px");
                            else
                                imgSettings.Add("margin-right", "-" + keyValuePair[1] + "px");

                            break;
                        case "my":
                            if (keyValuePair[1].StartsWith("-"))
                                imgSettings.Add("margin-bottom", keyValuePair[1].Replace("-", "") + "px");
                            else
                                imgSettings.Add("margin-bottom", "-" + keyValuePair[1] + "px");
                            break;
                    }
                }
            }
            
            return null;
        }

        public static string DecodeImageName(string imagecode)
        {
            if (imagecode.Contains("..."))
                return imagecode.Split(new string[] { "..." }, StringSplitOptions.RemoveEmptyEntries)[0] + "." + imagecode.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[imagecode.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
            else
                return imagecode;
        }

        public static void SaveImage(byte[] data, List<string> format, string name)
        {
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");

            string imageName = name.Substring(0, name.LastIndexOf("."));
            string imageExtension = name.Split('.')[name.Split('.').Length - 1];

            XMLImage xmlImage = new XMLImage();
            xmlImage.Data = data;
            xmlImage.Extension = imageExtension;
            xmlImage.Format = format;

            XmlSerializer ser = new XmlSerializer(typeof(XMLImage));
            StreamWriter writer = new StreamWriter("Images\\" + name + ".xml");
            ser.Serialize(writer, xmlImage);
            writer.Flush();
            writer.Close();
        }

        public XMLImage LoadImage(string imageName)
        {
             XMLImage image = new XMLImage();
            if (File.Exists(imageName + ".xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(XMLImage));
                StreamReader reader = new StreamReader(imageName + ".xml");
                image = (XMLImage)ser.Deserialize(reader);
                reader.Close();
            }
            else
            {

            }
            return image;
        }
    }
}
