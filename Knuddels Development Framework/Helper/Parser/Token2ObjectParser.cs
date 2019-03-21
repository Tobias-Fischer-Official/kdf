using System.Collections.Generic;
using System.Linq;
using KDF.ChatObjects.Collections;
using KDF.ChatObjects;
using System.Drawing;
using KDF.Exceptions;
using System;
using KDF.Networks.Core;

namespace KDF.Helper.Parser
{
    /// <summary>
    /// Parsed empfangene Token-Strings direkt in verwendbare Objekte
    /// </summary>
    /// <remarks>Diese Klasse wird nur für solche Objekte verwendet, die nicht bereits im Kontruktor der jeweiligen Klasse geparsed werden können, oder weitere Angaben aus der Client-Klasse benötigen</remarks>
    /// <seealso cref="WrongTokenException"/>
    /// <seealso cref="UserList"/>
    /// <seealso cref="ChannelUser"/>
    /// <seealso cref="KnuddelsClient"/>
    public class Token2ObjectParser
    {
        private KnuddelsClient _client;
        private const char _nullChar = '\0';

        /// <summary>
        /// Erstellt eine neu Instanz des Tokenparsers
        /// </summary>
        /// <param name="client">Der Client des Tokenparsers</param>
        public Token2ObjectParser(KnuddelsClient client) { _client = client; }

        /// <summary>
        /// Parsed eine globale Channelliste anhand des Tokestrings
        /// </summary>
        /// <param name="data">Das zu verwendende Token</param>
        /// <returns>Die globale Channelliste</returns>
        /// <remarks><paramref name="data"/> darf nur das 'b'-Token sein</remarks>
        /// <exception cref="WrongTokenException">Wird ein falsches Token übergeben, wird eine WrongTokenException ausgelöst</exception> 
        /// <exception cref="ParsingException">Wird das Token nicht korrekt geparsed, wird eine ParsingException ausgelöst</exception>
        public List<ChannellnGlobalList> ParseGlobalChannelList(string data)
        {
            try
            {
                if (data[0] != 'b')
                    throw new WrongTokenException(data.Split(_nullChar)[0]);

                List<ChannellnGlobalList> chList = new List<ChannellnGlobalList>();
                //Wir splitten den Channelstring in einzelne Channels auf
                //Die Channels werden durch ein "-" separiert
                string[] rawChannels = (data.Replace("b\0", String.Empty)).Split(new string[] { "\0-\0" }, StringSplitOptions.RemoveEmptyEntries);
                //z.B. Aachen\n40\0p\0B\0pics/icon_fullChannel.gif

                //Da ja immer eine Nummer an die Unterchannels gehangen wird,
                //müssen wir einen int dafür bereitstellen
                int lastChannelNumber = 1;
                string lastChannel = String.Empty;
                foreach (string rawChannel in rawChannels)
                {
                    //Ob der Channel mehrere Unterchannels hat
                    bool isParent = true;
                    //Der Hauptchannel über einem Subchannel
                    string parentChannel = "-";
                    //(komischerweise wird hier mit einem Unix-Zeilenumbruch getrennt, naja was solls)
                    string channelName = rawChannel.Split('\n')[0];
                    //Wenn der Channelname ein " ist, dann ist es ein Unterchannel eines vorrangegangen Channels.
                    if (channelName == "\"" && channelName.Length == 1)
                    {
                        isParent = false;
                        lastChannelNumber++;
                        parentChannel = lastChannel;
                        channelName = lastChannel + " " + lastChannelNumber.ToString();
                    }
                    //Manchmal gibt es z.B. den Channel Flirt 20 und dann erst Flirt 22.
                    //In diesem Fall steht die nummer hinter dem ", die wir natürlich dann auch setzen
                    else if (channelName.Contains("\""))
                    {
                        lastChannelNumber = int.Parse(channelName.Replace("\"", String.Empty));
                        channelName = lastChannel + " " + lastChannelNumber.ToString();
                    }
                    else
                    {
                        //Wenn nicht, ist es ein neuer Channel, und wir müssen die Nummer wieder auf 0 setzen.
                        lastChannel = channelName;
                        lastChannelNumber = 1;
                    }
                    //40
                    int usersOnlineInChannel = int.Parse(rawChannel.Split('\n')[1].Split(_nullChar)[0]);
                    //p = normaler Channel ohne Zugangsvorrausetzungen
                    //i = Channel mit Zugangsvorrausetzungen
                    bool isRestrictedChannel = false;
                    if (rawChannel.Split('\n')[1].Split(_nullChar)[1] == "i")
                        isRestrictedChannel = true;
                    //B
                    //Keine Ahnung, steht aber bei allen Channels dabei.

                    //pics/icon_fullChannel.gif = Ein Icon was hinter dem Channel dargestellt wird,
                    //können mehrere sein, daher eine Liste
                    List<string> channelImages = new List<string>();
                    //An der existenz des Bildes "icon_fullchannel.gif sehen wir on der Channel voll ist oder nicht.
                    bool isFull = false;
                    foreach (string str in rawChannel.Split('\n')[1].Split(_nullChar))
                    {
                        if (str.Contains(".gif"))
                        {
                            channelImages.Add(str);
                            if (str.Contains("icon_fullChannel.gif"))
                                isFull = true;
                        }
                    }
                    // Wir fügen unseren Channel der Liste hinzu
                    chList.Add(new ChannellnGlobalList(channelName, parentChannel, usersOnlineInChannel, isFull, channelImages, isRestrictedChannel, isParent));
                }
                return chList;
            }
            catch (Exception e)
            {
                ParsingException parsingException = new ParsingException(data, "ParseGlobalChannelList");
                parsingException.Data["origin"] = e;
                throw parsingException;
            }
        }

        /// <summary>
        /// Parsed eine Userliste anhand des Tokenstrings
        /// </summary>
        /// <param name="data">Das zu verwertende Token</param>
        /// <returns>Die geparste Userliste</returns>
        /// <remarks><paramref name="data"/> darf nur das 'u'-Token sein</remarks>
        /// <exception cref="WrongTokenException">Wird ein falsches Token übergeben, wird eine WrongTokenException ausgelöst</exception> 
        /// <exception cref="ParsingException">Wird das Token nicht korrekt geparsed, wird eine ParsingException ausgelöst</exception>
        public UserList ParseUserList(string data)
        {
            try
            {
                if (data[0] != 'u') throw new WrongTokenException(data.Split(_nullChar)[0]);

                UserList userList = new UserList(data.Split(_nullChar)[1]);
                List<string> users = new List<string>(data.Split(new string[] { "-" + _nullChar }, StringSplitOptions.RemoveEmptyEntries));                
                users[0].Replace(string.Format("u{0}{1}", _nullChar, userList.OwnerChannel), string.Empty);

                foreach (string user in users)
                {
                    string work = user;
                    if (work.StartsWith("u\0" + userList.OwnerChannel))
                        work = work.Replace("u\0" + userList.OwnerChannel, string.Empty);
                    string[] userParams = work.Split(new string[] { _nullChar.ToString() }, StringSplitOptions.RemoveEmptyEntries);
                    string userName = string.Empty;
                    int age = 0;
                    if (userParams[0].Contains('\n'))
                    {
                        userName = userParams[0].Split('\n')[0];
                        int.TryParse(userParams[0].Split('\n')[1], out age);
                    }
                    else
                        userName = userParams[0];
                    char format = ' ';
                    char.TryParse(userParams[1], out format);
                    Color foreColor = Color.Black;
                    if (userParams[2] != "B")
                        foreColor = Color.FromArgb(int.Parse(userParams[2].Split(',')[0]), int.Parse(userParams[2].Split(',')[1]), int.Parse(userParams[2].Split(',')[2]));
                    else if (userParams[2] == "B")
                        foreColor = Color.Black;
                    char gender = 'n';
                    List<string> images = new List<string>();
                    for (int x = 3; x <= userParams.Length - 1; x++)
                    {
                        images.Add(userParams[x]);
                        if (userParams[x].Contains("female"))
                            gender = 'w';
                        else if (userParams[x].Contains("male"))
                            gender = 'm';
                    }
                    userList.ChannelUserList.Add(new ChannelUser(userName, age, gender, foreColor, format, images));
                }
                return userList;
            }
            catch (Exception e)
            {
                ParsingException parsingException = new ParsingException(data, "ParseUserList");
                parsingException.Data["origin"] = e;
                throw parsingException;
            }
        }

        /// <summary>
        /// Wertet die Parameter eines Channels aus
        /// </summary>
        /// <param name="data">Das zu verwertende Token</param>
        /// <param name="change">Gibt an ob der Channel gewechselt wurde</param>
        /// <returns>Channel</returns>        
        /// <remarks>
        /// <para><paramref name="data"/> darf nur das '1' oder das 'a' Token sein</para>
        /// <para>Bei einem Channelwechsel werden einige Daten weniger übergeben, daher die Angabe, ob man den Channel gewechselt hat</para>
        /// </remarks>
        /// <exception cref="WrongTokenException">Wird ein falsches Token übergeben, wird eine WrongTokenException ausgelöst</exception>    
        /// <exception cref="ParsingException">Wird das Token nicht korrekt geparsed, wird eine ParsingException ausgelöst</exception>
        public Channel ParseChannel(string data, bool change)
        {
            try
            {
                if (data[0] != '1' && data[0] != 'a') throw new WrongTokenException(data.Split(_nullChar)[0]);

                string[] parameter = data.Split(_nullChar);
                string channelName = parameter[1];
                //Bei einem wechsel wird kein nick übergeben
                string nickJoined = string.Empty;
                if (!change)
                    nickJoined = parameter[2];
                //Bei dem Channel 8-ball bekommen wir ein array was 1 wert
                //mehr enthällt, bei 10 wird noch ein "-" eingeschoben
                //daher müssen wir alles was danach kommt um eins hochzählen
                //Ausserdem  wird bei einem wechsel weniger übergeben
                int x = 0;
                if (change)
                    x = -9;
                //wenn andere Channels auch solche probleme machen, haben wir damit eine
                //sehr dynamische Lösung für diese, einfach noch eine if-abfrage einfügen
                //kein Ärger bei Änderungen und Updates =)
                if (channelName.StartsWith("8")) { x += 1; }

                //Vordergrundfarbe des Channels
                Color foreColor = Color.FromArgb(int.Parse(parameter[x + 11].Split(',')[0]), int.Parse(parameter[x + 11].Split(',')[1]), int.Parse(parameter[x + 11].Split(',')[2]));
                //Hintergrundfarbe des Channels
                Color backColor = Color.FromArgb(int.Parse(parameter[x + 12].Split(',')[0]), int.Parse(parameter[x + 12].Split(',')[1]), int.Parse(parameter[x + 12].Split(',')[2]));
                parameter[x + 13] = parameter[x + 13].Replace("[", String.Empty);
                parameter[x + 13] = parameter[x + 13].Replace("]", String.Empty);
                parameter[x + 14] = parameter[x + 14].Replace("[", String.Empty);
                parameter[x + 14] = parameter[x + 14].Replace("]", String.Empty);
                //Wert durch den Rot ersetzt wird (°RR°)
                Color red = Color.Red;
                if (!(parameter[x + 13] == "R"))
                    red = Color.FromArgb(int.Parse(parameter[x + 13].Split(',')[0]), int.Parse(parameter[x + 13].Split(',')[1]), int.Parse(parameter[x + 13].Split(',')[2]));

                //Wert durch den Blau ersetzt wird (°BB°)
                Color blue = Color.Blue;
                if (!(parameter[x + 14] == "B"))
                    blue = Color.FromArgb(int.Parse(parameter[x + 14].Split(',')[0]), int.Parse(parameter[x + 14].Split(',')[1]), int.Parse(parameter[x + 14].Split(',')[2]));

                //Eine bisher unbekannte Farbe
                Color someOtherColor = Color.FromArgb(int.Parse(parameter[x + 18].Split(',')[0]), int.Parse(parameter[x + 18].Split(',')[1]), int.Parse(parameter[x + 18].Split(',')[2]));
                //Die Standartschriftgröße des Channels
                int fontSize = int.Parse(parameter[x + 15]);
                //Der horizontale Zeilenabstand des Cahnnels

                int linePitch = int.Parse(parameter[x + 16]);
                //Bei fotoflirt ist das linepitch immer 0
                if (channelName.Contains("Foto"))
                    linePitch = 0;
                //Wahrscheinlich die Zeit in der das interne Anti-Spam-Script anläuft
                //Wenn man nicht diese Zeit abstand zwischen zwei Posts lässt
                int maybeSpamTimeout = 3000;
                if (!change)
                    maybeSpamTimeout = int.Parse(parameter[x + 22]);
                //Wir erstellen einen neuen Channel anhand der Infos die uns der Server gesendet hat
                Channel chl = new Channel(channelName, nickJoined,foreColor, backColor, red, blue, someOtherColor, fontSize, linePitch, maybeSpamTimeout);
                //Wenn ein Hintergrundbild eingestellt ist (kein "-" an Stelle 8 des Arrays)
                if (parameter[8].Split('/')[parameter[8].Split('/').Length - 1] != "-" && !change)
                    chl.BackgroundImage = parameter[8];
                //Ob das Hintergrundbild gestreckt, zentriert oder wiederholt wird(?)
                string repeatStrechCenter = parameter[9];
                //Wir geben den Channel zurück
                return chl;
            }
            catch (Exception e)
            {
                ParsingException parsingException = new ParsingException(data, "ParseChannel");
                parsingException.Data["origin"] = e;
                throw parsingException;
            }
        }

        /// <summary>
        /// Erstellt ein neues ChannelUser-Objekt aus dem Tokenstring
        /// </summary>
        /// <param name="data">Das zu verwertende Token</param>
        /// <returns>Ein neues Objekt der ChannelUser-Klasse</returns>
        /// <exception cref="ParsingException">Wird das Token nicht korrekt geparsed, wird eine ParsingException ausgelöst</exception>
        public ChannelUser ParseUser(string data)
        {
            try
            {
                string[] userParameters = data.Split(_nullChar);

                string userName = string.Empty;
                int age = 0;
                if (userParameters[2].Contains("\n"))
                {
                    age = int.Parse(userParameters[2].Split('\n')[1]);
                    userName = userParameters[2].Split('\n')[0];
                }
                else userName = userParameters[2];

                char fontFormat = char.Parse(userParameters[3]);
                Color userForeColor = Color.FromArgb(int.Parse(userParameters[4].Split(',')[0]), int.Parse(userParameters[4].Split(',')[1]), int.Parse(userParameters[4].Split(',')[2]));

                List<string> userImages = new List<string>();
                char gender = 'n';
                int y = 0;
                for (int x = 5; x <= userParameters.Length - 1; x++)
                {
                    if (userParameters[x] == "-")
                    {
                        y = x + 1;
                        break;
                    }
                    gender = userParameters[x].Contains("female") ? 'w' : (userParameters[x].Contains("male") ? 'm' : 'n');
                    userImages.Add(userParameters[x]);
                }

                string channelLeaved = null;
                //Der Channel der verlassen wurde
                channelLeaved = userParameters[y] != "" ? userParameters[y] : null;

                string channelJoined = null;
                //Der Channel der betreten wurde
                channelJoined = userParameters[1] != "-" ? userParameters[1] : null;

                ChannelUser usr = new ChannelUser(userName, age, gender, userForeColor, fontFormat, userImages);

                usr.ChannelLeaved = channelLeaved;
                usr.ChannelJoined = channelJoined;

                //zwei bisher nicht bekannte Informationen
                string unknownArgument1 = userParameters[userParameters.Length - 2];
                string unknownArgument2 = userParameters[userParameters.Length - 1];

                return usr;
            }
            catch (Exception e)
            {
                ParsingException parsingException = new ParsingException(data, "ParseUser");
                parsingException.Data["origin"] = e;
                throw parsingException;
            }
        }

        /// <summary>
        /// Parsed einen User, welcher einen Channel verlassen hat
        /// </summary>
        /// <param name="packets">Das zu verwertende Token</param>
        /// <returns>Ein neues Objekt der UserLeftChannel-Klasse</returns>
        /// <remarks>
        /// <para><paramref name="packets"/> darf nur das 'w'-Token als string[] sein</para>        
        /// </remarks>
        /// <exception cref="WrongTokenException">Wird ein falsches Token übergeben, wird eine WrongTokenException ausgelöst</exception>  
        /// <exception cref="ParsingException">Wird das Token nicht korrekt geparsed, wird eine ParsingException ausgelöst</exception>
        public UserLeftChannel ParseUserLeftChannel(string[] packets)
        {
            try
            {
                if (packets[0] != "w") throw new WrongTokenException(packets[0]);
                UserLeftChannel ulc = null;
                ulc = packets.Length >= 5 ? new UserLeftChannel(packets[1], packets[2], packets[3], packets[4], packets[5]) : new UserLeftChannel(packets[1], packets[2], packets[3], packets[4]);
                return ulc;
            }
            catch (Exception e)
            {
                ParsingException parsingException = new ParsingException(string.Join("\0", packets), "ParsParseUserLeftChanneleUser");
                parsingException.Data["origin"] = e;
                throw parsingException;
            }
        }

        /// <summary>
        /// Parsed ein UserListen Bild
        /// </summary>
        /// <param name="packets">Das zu verwertende Token</param>
        /// <returns>Ein neues Objekt der UserListImage-Klasse</returns>
        /// <remarks>
        /// <para><paramref name="packets"/> darf nur das 'm' oder 'z' Token als array sein</para>        
        /// </remarks>
        /// <exception cref="WrongTokenException">Wird ein falsches Token übergeben, wird eine WrongTokenException ausgelöst</exception> 
        /// <exception cref="ParsingException">Wird das Token nicht korrekt geparsed, wird eine ParsingException ausgelöst</exception>
        public UserListImage ParseUserListImage(string[] packets)
        {
            try
            {
                if (packets[0] != "m" && packets[0] != "z") throw new WrongTokenException(packets[0]);
                return new UserListImage(packets[2], packets[1], packets[3]);
            }
            catch (Exception e)
            {
                ParsingException parsingException = new ParsingException(string.Join("\0", packets), "ParseUserListImage");
                parsingException.Data["origin"] = e;
                throw parsingException;
            }
        }

    }
}
