using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KDF.HelperClasses
{
    public class ExtremeRandom
    {
        public static int Gen(int min, int max)
        {
            int microtime = int.Parse(DateTime.Now.Ticks.ToString().Substring(14));
            int RandSeed = new Random(DateTime.Now.Millisecond).Next(int.MinValue, microtime);
            Thread.Sleep(1);
            int RandSeed2 = new Random(RandSeed).Next(int.MinValue, int.MaxValue);
            return new Random(RandSeed2).Next(min, max);
        }
    }
}
