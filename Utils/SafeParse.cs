using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
namespace AnotherRpgMod.Utils
{
    public static class FloatExtention
    {
        public static float SafeFloatParse(this string input)
        {
            if (String.IsNullOrEmpty(input)) { throw new ArgumentNullException("input"); }
            input = input.Replace(',', '.');
            float res;
            if (Single.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out res))
            {
                return res;
            }
            if (Single.TryParse(input, out res))
            {
                return res;
            }
            throw new ArgumentException("Fail To Parse");
        }

    }
}
