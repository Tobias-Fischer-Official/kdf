using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KDF.ChatObjects;
using System.Drawing;
using System.Diagnostics;

namespace KDF.Helper.Parser
{
    /// <summary>
    /// Stellt Methoden zur Umwandlung des Knuddels-Codes nach HTML bereit
    /// </summary>
    public class KCode2HTML
    {
        /// <summary>
        /// Hier werden K-Codes (Textkügelchen) in HTML übersetzt.
        /// </summary>
        /// <param name="c">Der betreffende Channel, aus welchem die Nachricht kam</param>
        /// <param name="function">Ob die Nachricht eine Funktion ist</param>
        /// <param name="row">Die Nachricht an sich</param>
        /// <returns>HTML-Code des K-Codes</returns>
        public static string ToHTML(string row, Channel c, bool function, bool useBB)
        {
            Color BB = Color.Blue;
            Color RR = Color.Red;
            Color FC = Color.Black;
            int? Size = 12;

            BB = c.Color1;
            RR = c.Color2;

            int counter = 0;
            List<KCodeItem> items = new List<KCodeItem>();
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == '°')
                {
                    if (i > 0)
                    {
                        if (row[i - 1] == '\\')
                            continue;
                    }
                    if (row.Length - 1 <= i)
                        break;
                    string sub = row.Substring(row.IndexOf('°') + 1);
                    if (!sub.Contains('°'))
                        break;
                    else
                    {
                        int index = i + sub.IndexOf('°') + 2;
                        string code = row.Substring(i, index - i);
                        row = row.Replace(code, "\n" + counter + "\n");
                        code = code.Substring(1, code.Length - 2);
                        KCodeItem tmpItem = new KCodeItem(counter, code, c, function);
                        items.Add(tmpItem);
                        counter++;
                        i += 2;
                    }
                }
            }

            KCode2HTML.ReplaceTags(ref row, "_", "<b>", "</b>");
            KCode2HTML.ReplaceTags(ref row, "\"", "<i>", "</i>");
            KCode2HTML.ReplaceTags(ref row, "#", "<br>");
            KCode2HTML.ReplaceTags(ref row, "§", "</span><span style=\"font-size:" + Size + "px; color: #" + ColorTranslator.ToHtml(FC) + ";\"></b></i>");
            KCode2HTML.ReplaceTags(ref row, "\\", "");
            KCode2HTML.ReplaceTags(ref row, "\\<", "&lt;");
            KCode2HTML.ReplaceTags(ref row, "\\>", "&gt;");



            #region Get HTML for °
            foreach (KCodeItem item in items)
                row = row.Replace("\n" + item.Id + "\n", item.HTML);
            #endregion
            return row;
        }

        /// <summary>
        /// Ersetzt einzelne Zeichen/Knuddels-Code durch ihre HTML-Definitionen
        /// </summary>
        /// <param name="row">Die betreffende Zeile</param>
        /// <param name="code">Der Knuddels-Code der ersetzt werden soll</param>
        /// <param name="firstTag">Das öffnende HTML-Tag</param>
        /// <param name="secondTag">Das schließende HTML-Tag</param>
        private static void ReplaceTags(ref string row, string code, string firstTag, string secondTag)
        {
            bool firstFound = false;
            for (int i = 0; i < row.Length; i++)
                if (row[i].ToString() == code)
                    if (i > 0 && row[i - 1] == '\\')
                        continue;
                    else
                    {
                        row = row.Remove(i, 1);
                        if (firstFound)
                            row = row.Insert(i, secondTag);
                        else
                            row = row.Insert(i, firstTag);
                        firstFound = !firstFound;
                    }
        }

        /// <summary>
        /// Ersetzt einzelne Zeichen/Knuddels-Code durch ihre HTML-Definitionen
        /// </summary>
        /// <param name="row">Die betreffende Zeile</param>
        /// <param name="code">Der Knuddels-Code</param>
        /// <param name="tag">Das einzufügende HTML-Tag</param>
        private static void ReplaceTags(ref string row, string code, string tag)
        {
            for (int i = 0; i < row.Length; i++)
                if (row[i].ToString() == code)
                    if (i > 0 && row[i - 1] == '\\')
                        continue;
                    else
                    {
                        row = row.Remove(i, 1);
                        row = row.Insert(i, tag);
                    }
        }

        private static string ImageHTMLBuilder(string imagecode)
        {
            string[] imgs = imagecode.Split(new string[] { "<>" }, StringSplitOptions.RemoveEmptyEntries);

            imgs[0] = imgs[0].Replace(">", "");
            if (imgs.Length == 2)
                imgs[1] = imgs[1].Replace("<", "");
            else
                imgs[0] = imgs[0].Replace("<", "");

            List<string> ImgsReady = new List<string>();
            List<Dictionary<string, string>> ImgsReadyFormat = new List<Dictionary<string, string>>();

            foreach (string img in imgs)
            {
                string name = ImageHelper.DecodeImageName(img);
                Dictionary<string, string> imgSettings = new Dictionary<string, string>();                
                string[] imgContainerParameter = imagecode.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in imgContainerParameter)
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

                ImgsReady.Add(name);
                ImgsReadyFormat.Add(imgSettings);
            }

            StringBuilder html = new StringBuilder();
            html.Append("<span style=\"overflow:visible; ");
            foreach (KeyValuePair<string, string> kvp in ImgsReadyFormat[0])
                html.Append(" " + kvp.Key + ":" + kvp.Value + ";");

            int imgFirst = 0;
            if (ImgsReady.Count > 1)
                imgFirst = 1;

            html.Append("\"><img src=\"http://chat.knuddels.de/pics/" + ImgsReady[imgFirst] + "\" style=\"margin-left:auto; margin-right:auto; ");
            if (ImgsReady.Count > 1)
                html.Append("\" background:url(http://chat.knuddels.de/pics/" + ImgsReady[0] + ") no-repeat;\">");
            else
                html.Append("\">");
            html.Append("</span>");
        
            //Debug.WriteLine(html);
            //Debug.WriteLine(code);

            return html.ToString();
        }

        /// <summary>
        /// Die Klasse mit der der K-Code in HTML geparsed wird
        /// </summary>
        private class KCodeItem
        {
            private int id;
            private string code;
            private string html;
            //Eine Sammlung von Regular-Expressions für die Identifizierung der Knuddels-Links
            private Dictionary<string, string> regExCollection = new Dictionary<string, string>();

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="id">Die Nummer des Code-Elements</param>
            /// <param name="code">Der Inhalt des Code-Elements</param>
            /// <param name="c">Der Channel</param>
            /// <param name="function">Ob der Text als funktion geparsed werden soll (blaue links)</param>
            public KCodeItem(int id, string code, Channel c, bool function)
            {
                this.id = id;
                this.code = code;

                if (code.Length > 0 && code[0] == '>' || code.StartsWith("BB >") || code.StartsWith("BB>") && code[1] != '{')
                {
                    string fontColor = ColorTranslator.ToHtml(c.ForeColor);
                    string fontWeight = "'normal'";
                    string textDecoration = "";

                    if (code.StartsWith("BB"))
                    {
                        fontColor = ColorTranslator.ToHtml(c.Color1);
                        textDecoration = "underline";
                    }
                    if (code.Contains('_'))
                        fontWeight = "bold";

                    string specialFormat = " style=\"color: rgb(" + fontColor + "); font-weight:" + fontWeight + "; text-decoration: " + textDecoration + "\"";

                    string[] param = null;
                    string leadingImage = string.Empty;

                    //Klickbarer Link
                    if (code.Contains("|"))
                    {
                        //Debug.WriteLine(code);
                        if (code.Contains("<>"))
                        {
                            string[] temp = code.Split(new string[] { "<>" }, StringSplitOptions.RemoveEmptyEntries);
                            param = temp[1].Split('|');
                            leadingImage = temp[0];
                            leadingImage = ImageHTMLBuilder(leadingImage);
                        }
                        else
                            param = code.Split('|');

                        //Debug.WriteLine(string.Join(",", param));

                        string content = param[0];

                        string classes = string.Empty;
                        if (content.Contains("_h"))
                            classes += "hoverUnderline ";

                        string stylesheet = string.Empty;
                        if (content.Contains("BB") || function)
                            stylesheet += "color:" + ColorTranslator.ToHtml(c.Color2) + ";";
                        else
                            stylesheet += "color:" + ColorTranslator.ToHtml(c.ForeColor) + ";";

                        content = content.Replace("_h", "");
                        content = content.Replace("BB>", "");
                        content = content.Replace("BB >", "");
                        content = content.Replace(">", "");

                        string command1 = param.Length >= 2 ? param[1] : null;
                        if (command1 != null)
                            command1.Replace("\"", content);

                        string command2 = param.Length >= 3 ? param[2] : command1;

                        string linkHtmlCommand = " name=\"" + command1 + "|" + command2 + "\" ";

                        this.html = "<a style=\"" + stylesheet + "\" class=\"" + classes + "\" href=\"#\" " + linkHtmlCommand + ">" + content + "</a>";
                    }
                    else
                    {
                        this.html = ImageHTMLBuilder(code);
                    }

                    if (this.html == null || this.html == "" || this.html == string.Empty)
                        this.html = code;
                }
                #region RGB Color
                else if (code.Length != 0 && code[0] == '[')
                {
                    try
                    {
                        code = code.Substring(1);
                        code = code.Remove(code.Length - 1);
                        string[] rgb = code.Split(new char[] { ',' });
                        Color color = Color.FromArgb(
                            int.Parse(rgb[0]),
                            int.Parse(rgb[1]),
                            int.Parse(rgb[2]));
                        this.html =
                            "<span style=\"color: " +
                            ColorTranslator.ToHtml(color) +
                            ";\">";
                    }
                    catch
                    {

                    }
                }
                #endregion
                #region ColorLetter, Size, Fehler
                else if (!code.StartsWith("%"))
                {  //ColorLetter / Size

                    string[] ChatColors = { "W", "E", "R", "O", "P", "A", "D", "G", "K", "Y", "C", "B", "N", "M", "BB", "RR" };
                    string[] ChatSizes = new string[31];
                    for (int i = 1; i <= 30; i++)
                        ChatSizes[i] = i.ToString();

                    string[] HTMLColors = { "#FFFFFF", "#00AC00", "#FF0000", "#FFC800", "#FFAFAF", "#808080", "#404040", "#00FF00", "#000000", "#FFFF00", "#00FFFF", "#0000FF", "#964A00", "#FF00FF", ColorTranslator.ToHtml(c.Color1).Replace("#", ""), ColorTranslator.ToHtml(c.Color2).Replace("#", "") };

                    #region Check Colors
                    for (int i = 1; i <= 16; i++)
                        if (code == ChatColors[i - 1])
                        {
                            this.html = "<span style=\"color: " + HTMLColors[i - 1] + ";\">";
                            return;
                        }
                    #endregion

                    #region CheckFormatReset
                    if (code == "r")
                    {
                        this.html = "</span></b></i><span style=\"font-size:" + c.FontSize + "px; color: " + ColorTranslator.ToHtml(c.ForeColor) + ";\">";
                        return;
                    }

                    #endregion

                    #region Check Points
                    if (code == "!")
                    {
                        this.html = "<span style=\"letter-spacing: 2px; color: " + ColorTranslator.ToHtml(c.ForeColor) + "; font-size:7px;\">.........</span>";
                        return;
                    }
                    #endregion

                    #region Check Sizes
                    try
                    {
                        int.Parse(code);
                        this.html = "<span style=\"font-size:" + code + "px\">";
                        return;
                    }
                    catch { }
                    #endregion

                    #region Size&Color
                    try
                    {
                        string re1 = "(\\d+)";	// Integer Number 1
                        string re2 = "([a-z])";	// Any Single Character 1

                        string size = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(code).Groups[1].ToString();
                        string color = new Regex(re2, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(code).Groups[1].ToString();

                        for (int i = 1; i <= 16; i++)
                            if (color == ChatColors[i - 1])
                                color = HTMLColors[i - 1];

                        this.html = "<span style=\"font-size:" + size + "px; color: " + color + ";\">";
                        return;
                    }
                    catch{ /*Fehler hihi*/ }
                    #endregion

                    this.html = code;
                    //Konnte nichts mit anfangen, also erstmal zurück,
                    //damit man sieht, wenn Fehler auftauchen.
                }

                #endregion
            }

            public int Id
            {
                get { return this.id; }
            }
            public string Code
            {
                get { return this.code; }
            }
            public string HTML
            {
                get { return this.html; }
            }
        }
    }
}