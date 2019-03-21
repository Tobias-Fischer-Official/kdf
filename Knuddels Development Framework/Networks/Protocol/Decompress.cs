using System;
using System.Text;

namespace KDF.Networks.Protocol
{
    internal class Decompress
    {
        private Object aObject;
        private Object[] b = new Object[2];
        private long c = 0L;
        private long d = 0L;
        private bool e;
        private byte[] f;
        private int g = 0;
        private int h = 0;

        internal Decompress(String paramString)
        {
            a(this.b, paramString);
        }

        private void a(Object[] paramArrayOfObject, String paramString)
        {
            int i = 0;
            int j = paramString.Length;
            int m = 1;
            int n = -33;
            int i1 = 0;
            int i2 = 0;
            while (i < j)
            {
                int k = paramString[i];
                if (k == 255)
                {
                    i2 = paramString[i + 1] + 1;
                    i1 = paramString[i + 2];
                    i += 2;
                }
                else
                {
                    i2 = k / 21 + 1;
                    i1 = k % 21;
                }
                if ((m & 0x1) == 0)
                {
                    m++;
                    while (n < i1)
                    {
                        m <<= 1;
                        n++;
                    }
                }
                else
                {
                    do
                    {
                        m >>= 1;
                        n--;
                    }
                    while ((m & 0x1) == 1);
                    m++;
                    while (n < i1)
                    {
                        m <<= 1;
                        n++;
                    }
                }
                int i3 = a(m, i1);
                int x = paramString.Length;
                String str = paramString.Substring(i + 1, i2);
                if ((i2 == 3) && (str.Equals("\\\\\\")))
                    this.aObject = str;
                a(paramArrayOfObject, str, i3, i1);
                i += i2 + 1;
            }
        }

        internal String Run(byte[] paramArrayOfByte)
        {
            if (paramArrayOfByte == null)
                return null;

            StringBuilder localStringBuffer = new StringBuilder(paramArrayOfByte.Length * 100 / 60);
            this.f = paramArrayOfByte;
            this.g = 0;
            this.h = 0;
            this.e = false;
            Object[] arrayOfObject = this.b;
            while (!this.e)
            {
                arrayOfObject = (Object[])arrayOfObject[a()];
                if (arrayOfObject[0] != null)
                    continue;
                if (arrayOfObject[1] == this.aObject)
                {
                    int i = 0;
                    for (int j = 0; j < 16; j++)
                        i += (a() << j);
                    localStringBuffer.Append((char)i);
                }
                else
                {
                    localStringBuffer.Append((String)arrayOfObject[1]);
                }
                arrayOfObject = this.b;
            }
            String str = localStringBuffer.ToString();
            this.d += str.Length;
            this.c += paramArrayOfByte.Length;
            return str;
        }

        private bool a(Object[] paramArrayOfObject, String paramString, int paramInt1, int paramInt2)
        {
            if (paramInt2 == 0)
            {
                paramArrayOfObject[1] = paramString;
                if (paramArrayOfObject[0] != null)
                    return false;
            }
            else
            {
                if (paramArrayOfObject[0] == null)
                {
                    if (paramArrayOfObject[1] != null)
                        return false;
                    paramArrayOfObject[0] = new Object[2];
                    paramArrayOfObject[1] = new Object[2];
                }
                return a((Object[])paramArrayOfObject[(paramInt1 & 0x1)], paramString, paramInt1 >> 1, paramInt2 - 1);
            }
            return true;
        }

        private int a()
        {
            int i = 0;
            if ((this.f[this.h] & 1 << this.g) != 0)
                i = 1;
            this.g += 1;
            if (this.g > 7)
            {
                this.g = 0;
                this.h += 1;
                this.e = (this.h == this.f.Length);
            }
            return i;
        }

        private int a(int paramInt1, int paramInt2)
        {
            int i = 0;
            int j = 1;
            int k = 1 << paramInt2 - 1;
            while (paramInt2 > 0)
            {
                if ((paramInt1 & j) != 0)
                    i += k;
                j <<= 1;
                k >>= 1;
                paramInt2--;
            }
            return i;
        }
    }
}
