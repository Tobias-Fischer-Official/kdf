using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace KDF.Networks.Protocol.Module
{
    public class KnModule
    {
        #region private Vars
        private Hashtable i;
        private Hashtable g;
        private int id;
        private Hashtable values;
        private String protocolHash;
        private ArrayList names;
        private ArrayList f;
        private int startValue;
        private string protocolString;
        private int protocolIndex;
        private ArrayList n;
        #endregion

        #region public Vars
        public String Name { get { return (String)names[this.id]; } }
        public Int32 ID { get { return this.id; } }
        public Hashtable Values { get { return values; } }
        public String Hash { get { return protocolHash; } }
        #endregion

        #region Konstruktor
        private KnModule() { }

        private KnModule(int id)
        {
            this.id = id;
            this.values = new Hashtable();
        }
        #endregion

        #region GetValue
        public T GetValue<T>(string key)
        {
            if (values == null)
                return default(T);
            Object value = values[key];
            if (value == null)
                return default(T);

            return (T)value;
        }

        public int GetValue(string pName, string pKey)
        {
            if (g == null)
                return -1;

            if (!g.ContainsKey(pName))
                return -1;

            Hashtable table = (Hashtable)g[pName];
            
            if (!table.ContainsKey(pKey))
                return -1;
            return int.Parse(table[pKey].ToString());
        }

        public Dictionary<string, object> GetKeyValue()
        {
            try
            {
                Dictionary<string, object> keyValueDic = new Dictionary<string, object>();
                ArrayList keyValue = GetValue<ArrayList>("KEY_VALUE");
                foreach (KnModule kvPair in keyValue)
                    keyValueDic.Add(kvPair.GetValue<string>("KEY"), kvPair.GetValue<object>("VALUE"));
                return keyValueDic;
            }
            catch
            {
            }
            return new Dictionary<string, object>();
        }

        private object GetValue(object key)
        {
            return this.values[key];
        }
        #endregion

        #region Create
        private KnModule Create(int id)
        {
            return new KnModule(id)
            {
                g = this.g,
                protocolHash = this.protocolHash,
                names = this.names,
                f = this.f,
                i = this.i,
            };
        }
        
        public KnModule CreateModule(string name)
        {
            return this.Create((Int32)this.g[name]);
        }
        #endregion

        #region Read
        public KnModule Parse(byte[] buffer)
        {
            PacketReader dataInput = new PacketReader(buffer);
            KnModule result;
            try
            {
                short id = (short)dataInput.ReadShort();
                KnModule xb = this.Create(id);
                this.Read(xb, dataInput, id, xb);
                result = xb;
            }
            catch
            {
                return null;
            }
            return result;
        }

        public KnModule Parse(string packet)
        {
            return Parse(Encoding.Default.GetBytes(packet.Substring(2)));
        }

        private object Read(KnModule xb, PacketReader dataInput, int id, KnModule xb2)
        {
            if (xb2 == null)
            {
                xb2 = this.Create(id);
            }

            ArrayList arrayList = (ArrayList)this.f[id];

            for (int i = 0; i < arrayList.Count; i++)
            {
                Int32 integer = (Int32)arrayList[i];
                switch (integer)
                {
                    case 0: // Byte 
                        return dataInput.Read();
                    case 1: // Boolean 
                        return dataInput.ReadBoolean();
                    case 2: // Byte 
                        return dataInput.Read();
                    case 3: // Short 
                        return dataInput.ReadShort();
                    case 4: // Int32 
                        return dataInput.ReadInt();
                    case 5: // Long 
                        return dataInput.ReadLong();
                    case 6: // Float 
                        return (float)dataInput.ReadInt();
                    case 7: // Double 
                        return BitConverter.Int64BitsToDouble(dataInput.ReadLong());
                    case 8: // Char 
                        return dataInput.ReadChar();
                    case 9: // String 
                        String str = dataInput.ReadUTF();
                        if (str == null || str.Length == 0 || str[0] != 0)
                            return str;
                        if (str.Length == 1)
                            return null;
                        return str.Substring(1);
                    case 10: // BinaryTree 
                        break;
                    case 11: // Array (Start) 
                        i++;
                        integer = (Int32)arrayList[i];
                        string text = (string)this.names[integer];
                        ArrayList arrayList2 = new ArrayList();
                        xb2.b(text, arrayList2);
                        while ((sbyte)dataInput.Read() == 11)
                        {
                            object obj = this.Read(xb, dataInput, integer, null);
                            obj = this.a(xb, integer, obj);
                            arrayList2.Add(obj);
                        }
                        i++;
                        break;
                    case 12: // Array (End) 
                        break;
                    case 13: // String 
                        int len = dataInput.Read();
                        if (len == 255) return null;
                        if (len >= 128) len = len - 128 << 16 | (dataInput.Read()) << 8 | (dataInput.Read());
                        StringBuilder builder = new StringBuilder(len + 2);
                        for (int _i = 0; _i < len; _i++)
                            builder.Append((char)dataInput.Read());

                        return builder.ToString();
                    default:
                        string text2 = (string)this.names[integer];
                        object obj2 = this.Read(xb, dataInput, integer, null);
                        obj2 = this.a(xb, integer, obj2);
                        xb2.b(text2, obj2);
                        break;
                }
            }
            return xb2;
        }

        private object a(KnModule xb, Int32 integer, object obj)
        {
            object obj2 = (this.i != null) ? this.i[integer] : null;
            if (obj2 == null)
                return obj;

            if (obj == null)
                xb.a(integer, obj);

            return obj;
        }
        #endregion

        #region Write
        public byte[] WriteBytes(KnModule xb)
        {
            byte[] result;
            try
            {
                int num = xb.id;
                PacketWriter writer = new PacketWriter();
                writer.WriteShort((short)num);
                this.Write(xb, num, writer);
                result = Encoding.Default.GetBytes(writer.ToString());
            }
            catch
            {
                return null;
            }
            return result;
        }

        private void Write(object obj, int id, PacketWriter dataOutput)
        {
            ArrayList arrayList = (ArrayList)this.f[id];
            for (int i = 0; i < arrayList.Count; i++)
            {
                int num = (Int32)arrayList[i];
                switch (num)
                {
                    case 0:
                        dataOutput.Write((int)obj);
                        break;
                    case 1:
                        dataOutput.WriteBoolean((bool)obj);
                        break;
                    case 2:
                        dataOutput.Write((byte)obj);
                        break;
                    case 3:
                        dataOutput.WriteShort((short)obj);
                        break;
                    case 4:
                        dataOutput.WriteInt((int)obj);
                        break;
                    case 5:
                        dataOutput.WriteLong((long)obj);
                        break;
                    case 6:
                        dataOutput.WriteFloat((float)obj);
                        break;
                    case 7:
                        dataOutput.WriteDouble((double)obj);
                        break;
                    case 8:
                        dataOutput.WriteChar((int)obj);
                        break;
                    case 9:
                        string s = (string)obj;
                        if (s == null || s.Length == 0 || s[0] != 0)
                        {
                            dataOutput.WriteUTF(s);
                            break;
                        }

                        if (s.Length == 1)
                        {
                            dataOutput.WriteUTF(null);
                            break;
                        }

                        dataOutput.WriteUTF(s.Substring(1));
                        break;
                    case 10:
                        throw new Exception("Not implemented yet: BinaryType");
                    case 11:
                        i++;
                        num = (Int32)arrayList[i];

                        string text = (string)this.names[num];
                        ArrayList arrayList2 = (ArrayList)((KnModule)obj).GetValue(text);

                        if (arrayList2 != null)
                        {
                            for (int j = 0; j < arrayList2.Count; j++)
                            {
                                dataOutput.Write(11);
                                this.Write(arrayList2[j], num, dataOutput);
                            }
                        }
                        dataOutput.Write(12);
                        i++;
                        break;
                    case 12:
                        throw new Exception("Not expected: ArrayEnd");
                    case 13:
                        this.WriteString((string)obj, dataOutput);
                        break;
                    default:
                        this.Write(((KnModule)obj).GetValue(this.names[num]), num, dataOutput);
                        break;
                }
            }
        }

        private void WriteString(string text, PacketWriter dataOutput)
        {
            if (text == null)
            {
                dataOutput.Write(255);
            }
            else
            {
                int len = text.Length;
                if (len < 128)
                {
                    dataOutput.Write(len);
                }
                else
                {
                    if (len > 8388608)
                    {
                        throw new IOException("String too long: " + len.ToString());
                    }
                    dataOutput.Write((int)((uint)len >> 16 | 128u));
                    dataOutput.Write((int)((uint)len >> 8 & 255u));
                    dataOutput.Write(len & 255);
                }
                if (len > 0)
                {
                    dataOutput.WriteChars(text);
                }
            }
        }
        #endregion

        #region Add
        public KnModule Add(string name, object value)
        {
            return this.b(name, value);
        }
        
        private KnModule b(string text, object obj)
        {
            if (obj == null)
                this.values.Remove(text);
            else
                this.values.Add(text, obj);

            return this;
        }
        #endregion

        #region Tree
        public static KnModule StartUp(string moduleTree)
        {
            KnModule instance = new KnModule();
            instance.ParseTree(moduleTree);
            return instance;
        }

        public virtual void ParseTree(string str)
        {
            string text = ";";
            string text2 = ":";
            this.Set(str);
            this.protocolHash = this.GetString(text);
            this.startValue = this.ConvertToInt(text);
            int num = this.startValue;
            this.names = new ArrayList();
            while (!this.End(text))
            {
                this.Add(this.names, num++, this.GetString(text));
            }
            this.g = new Hashtable();
            int i;
            for (i = 0; i < this.names.Count; i++)
            {
                object obj = this.names[i];
                if (obj != null)
                {
                    this.g.Add(this.names[i], i);
                }
            }
            this.f = new ArrayList();
            i = this.startValue;
            while (!this.End(text2))
            {
                ArrayList arrayList = new ArrayList();
                this.Add(this.f, i, arrayList);
                while (!this.End(text))
                {
                    arrayList.Add(this.ConvertToInt(text));
                }
                i++;
            }
            int num2 = -1;
            while (!this.End(text))
            {
                num2 = this.b(num2);
                string text3 = (string)this.names[num2];
                Hashtable hashtable = new Hashtable();
                if (this.g.ContainsKey(text3))
                    this.g.Remove(text3);
                this.g.Add(text3, hashtable);
                int num3 = 0;
                while (!this.End(text))
                {
                    object temp = this.GetString(text);
                    num3++;
                    hashtable.Add(temp, (byte)num3);
                }
            }
            this.Set(null);
        }
        
        private void Add(ArrayList arrayList, int num, object obj)
        {
            while (arrayList.Count <= num)
            {
                arrayList.Add(null);
            }
            arrayList[num] = obj;
        }

        private string GetString(string text)
        {
            int index = this.protocolString.IndexOf(text, this.protocolIndex);
            if (index < 0)
            {
                return null;
            }
            string result = this.protocolString.Substring(this.protocolIndex, index - this.protocolIndex);
            this.protocolIndex = index + text.Length;
            return result;
        }

        private int ConvertToInt(string text)
        {
            try
            {
                string idString = this.GetString(text);
                return Int32.Parse(idString);
            }
            catch { return (int)text[0]; }
        }

        private void Set(string text)
        {
            this.protocolString = text;
            this.protocolIndex = 0;
        }

        private bool End(string text)
        {
            if (protocolString.IndexOf(text, protocolIndex) == protocolIndex)
            {
                this.protocolIndex += text.Length;
                return true;
            }
            return false;
        }

        private int b(int num)
        {
            Int32 obj = 0;
            do
            {
                num++;
                if (num >= this.f.Count)
                {
                    break;
                }
            }
            while (this.f[num] == null || ((ArrayList)this.f[num]).Count == 0 || !Object.Equals(((ArrayList)this.f[num])[0], obj));
            if (num < this.f.Count)
            {
                return num;
            }
            string exception = "Not enough enumeration rules found.";
            throw new Exception(exception);
        }

        private void a(Int32 integer, object obj)
        {
            if (this.n == null)
                this.n = new ArrayList();

            this.n.Add(integer);
            this.n.Add(obj);
        }
        #endregion
    } 
}
