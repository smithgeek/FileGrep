using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smithgeek
{
    public static class NumParser
    {
        /// <summary>
        /// Parses a string containing a hexadecimal number and returns an Int64
        /// </summary>
        public static Int64 ParseHexToInt64(String hexValue)
        {
            return ParseInt64(hexValue, true);
        }

        public static Int64 ParseInt64(String number)
        {
            return ParseInt64(number, number.ToUpper().StartsWith("0X"));
        }

        private static Int64 ParseInt64(String number, bool hex)
        {
            Int64 integer = 0;
            try
            {
                if (hex)
                {
                    if (number.ToUpper().StartsWith("0X"))
                    {
                        number = number.Substring(2);
                    }
                    integer = Int64.Parse(number, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    integer = Int64.Parse(number);
                }
            }
            catch (Exception)
            { }
            return integer;
        }

        /// <summary>
        /// Parses a string containing a hexadecimal number and returns an Int64
        /// </summary>
        public static int ParseHexToInt(String hexValue)
        {
            return ParseInt(hexValue, true);
        }

        public static int ParseInt(String number)
        {
            return ParseInt(number, number.ToUpper().StartsWith("0X"));
        }

        private static int ParseInt(String number, bool hex)
        {
            int integer = 0;
            try
            {
                if (hex)
                {
                    if (number.ToUpper().StartsWith("0X"))
                    {
                        number = number.Substring(2);
                    }
                    integer = Int32.Parse(number, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    integer = Int32.Parse(number);
                }
            }
            catch (Exception)
            { }
            return integer;
        }

        /// <summary>
        /// Parses a string containing a hexadecimal number and returns an Int64
        /// </summary>
        public static UInt32 ParseHexToUint(String hexValue)
        {
            return ParseUint(hexValue, true);
        }

        public static UInt32 ParseUint(String number)
        {
            return ParseUint(number, number.ToUpper().StartsWith("0X"));
        }

        private static UInt32 ParseUint(String number, bool hex)
        {
            UInt32 integer = 0;
            try
            {
                if (hex)
                {
                    if (number.ToUpper().StartsWith("0X"))
                    {
                        number = number.Substring(2);
                    }
                    integer = UInt32.Parse(number, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    integer = UInt32.Parse(number);
                }
            }
            catch (Exception)
            { }
            return integer;
        }

        /// <summary>
        /// Parses a string containing a hexadecimal number and returns an Int64
        /// </summary>
        public static Double ParseHexToDouble(String hexValue)
        {
            return ParseDouble(hexValue, true);
        }

        public static Double ParseDouble(String number)
        {
            return ParseDouble(number, number.ToUpper().StartsWith("0X"));
        }

        private static Double ParseDouble(String number, bool hex)
        {
            Double dbl = 0;
            try
            {
                if (hex)
                {
                    if (number.ToUpper().StartsWith("0X"))
                    {
                        number = number.Substring(2);
                    }
                    for (int n = number.Length - 1; n >= 0; n--)
                    {
                        dbl += System.Uri.FromHex(number[n]) * Math.Pow(16, number.Length - 1 - n);
                    }
                }
                else
                {
                    dbl = Double.Parse(number);
                }
            }
            catch (Exception)
            { }
            return dbl;
        }
    }
}
