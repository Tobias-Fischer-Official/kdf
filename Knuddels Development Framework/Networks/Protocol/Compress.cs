using System;
using System.Collections;

namespace KDF.Networks.Protocol
{
    internal class Compress
    {
        private int aInt;
        private int bInt;
        private Hashtable cHashTable = new Hashtable(1, 1.0F);
        private long d = 0L;
        private long e = 0L;
        private byte[] f = new byte[65535];
        private int g = 0;
        private int h = 0;
        private char[] i;
        private short[] j;

        internal Compress(String paramString)
        {
            a();
            a(paramString);
        }

        internal byte[] Run(String paramString, int paramInt)
        {
            byte[] arrayOfByte1 = (byte[])null;
            if (paramInt > 0)
            {
                arrayOfByte1 = this.f;
                this.f = new byte[paramInt];
            }
            this.g = 0;
            this.h = 0;
            int k = paramString.Length;
            this.e += k;
            int i1 = 0;
            while (i1 < k)
            {
                Object localObject = null;
                int n = i1 + 1;
                Hashtable localHashtable = this.cHashTable;
                while (i1 < k)
                {
                    int m = paramString[i1];
                    int localInteger = (int)(localHashtable[b(m)] == null ? 0 : localHashtable[b(m)]);
                    if (localInteger != 0x00)
                    {
                        n = i1 + 1;
                        localObject = localInteger;
                    }
                    localHashtable = (Hashtable)localHashtable[c(m)];
                    if (localHashtable == null)
                        break;
                    i1++;
                }
                i1 = n;
                if (localObject == null)
                {
                    a(this.aInt);
                    a(268435456 + paramString[i1 - 1]);
                }
                else
                {
                    a((int)localObject);
                }
            }
            b();
            byte[] arrayOfByte2 = new byte[this.h];
            Array.Copy(this.f, 0, arrayOfByte2, 0, this.h);
            this.d += this.h;
            if (paramInt > 0)
                this.f = arrayOfByte1;
            return arrayOfByte2;
        }

        private void a(int paramInt)
        {
            int k = paramInt >> 24;
            int m = paramInt - (k << 24);
            if ((this.g != 0) && (k > 0))
            {
                k += this.g - 8;
                m <<= this.g;
                int tmp55_51 = (this.h++);
                byte[] tmp55_41 = this.f;
                tmp55_41[tmp55_51] = (byte)(tmp55_41[tmp55_51] + (byte)m);
                m >>= 8;
                this.g = 0;
            }
            while (k > 0)
            {
                this.f[(this.h++)] = (byte)m;
                m >>= 8;
                k -= 8;
            }
            if (k < 0)
            {
                this.h -= 1;
                this.g = (k + 8);
            }
        }

        private void a(string textToCompress)
        {
            int k = 0;
            int m = textToCompress.Length;
            int i1 = 1;
            int i2 = -33;
            int i3 = 0;
            int i4 = 0;
            while (k < m)
            {
                int n = textToCompress[k];
                if (n == 255)
                {
                    i4 = textToCompress[k + 1] + '\x001';
                    i3 = textToCompress[k + 2];
                    k += 2;
                }
                else
                {
                    i4 = n / 21 + 1;
                    i3 = n % 21;
                }
                if ((i1 & 0x1) == 0)
                {
                    i1++;
                    while (i2 < i3)
                    {
                        i1 <<= 1;
                        i2++;
                    }
                }
                else
                {
                    do
                    {
                        i1 >>= 1;
                        i2--;
                    }
                    while ((i1 & 0x1) == 1);
                    i1++;
                    while (i2 < i3)
                    {
                        i1 <<= 1;
                        i2++;
                    }
                }
                int localInteger = ab(i1, i3) + (i3 << 24);
                String str = textToCompress.Substring(k + 1, i4);
                if ((this.bInt == 0) && (i3 > 8))
                    this.bInt = (ab(i1 >> i3 - 8, 8) + 134217728);
                if ((i4 == 3) && (str.Equals("\\\\\\")))
                    this.aInt = localInteger;
                else
                    a(this.cHashTable, str, 0, localInteger);
                k += i4 + 1;
            }
        }

        private void a()
        {
            this.i = new char[256];
            this.j = new short[256];
            for (int k = 0; k < this.i.Length; k++)
            {
                this.i[k] = (char)k;
                this.j[k] = (short)k;
            }
        }

        private void a(Hashtable paramHashtable, String paramString, int paramInt, Object paramObject)
        {
            int k = paramString[paramInt];
            if (paramInt + 1 >= paramString.Length)
            {
                if (paramHashtable[b(k)] != null)
                    throw new Exception("Error beim baum bauen du noob: " + paramString);
                paramHashtable[b(k)] = paramObject;
            }
            else
            {
                Hashtable localHashtable = (Hashtable)paramHashtable[c(k)];
                if (localHashtable == null)
                {
                    localHashtable = new Hashtable(1, 1.0F);
                    paramHashtable[c(k)] = localHashtable;
                }
                a(localHashtable, paramString, paramInt + 1, paramObject);
            }
        }


        private int ab(int paramInt1, int paramInt2)
        {
            int k = 0;
            int m = 1;
            int n = 1 << paramInt2 - 1;
            while (paramInt2 > 0)
            {
                if ((paramInt1 & m) != 0)
                    k += n;
                m <<= 1;
                n >>= 1;
                paramInt2--;
            }
            return k;
        }       

        private char b(int paramInt)
        {
            paramInt &= 65535;
            return paramInt < 256 ? this.i[paramInt] : (char)paramInt;
        }

        private short c(int paramInt)
        {
            paramInt &= 65535;
            return paramInt < 256 ? this.j[paramInt] : (short)paramInt;
        }

        private void b()
        {
            int k = this.h;
            if (this.g != 0)
                while (k == this.h)
                    a(this.bInt);
        }
    }
}
