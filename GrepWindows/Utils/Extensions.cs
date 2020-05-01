using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Smithgeek.Extensions
{
    /// <summary>
    /// Extension methods for generic objects.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Alternative format to check if an object is null.
        /// </summary>
        /// <param name="obj">The object to check for null</param>
        /// <returns>True if the object is null</returns>
        public static bool isNull(this object obj)
        {
            bool isNull = null == obj;
            if (!isNull && obj.GetType() == typeof(String))
            {
                int length = ((String)obj).Length;
                if(length == 1)
                {
                    isNull = ((String)obj)[0] == '\0';
                }
            }
            return isNull;
        }

    }

    /// <summary>
    /// Extension methods for linq.
    /// </summary>
    public static class LinqExtensions
    {
        private static Random mRandom = null;
        /// <summary>
        /// Gets a random object from a collection
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection</typeparam>
        /// <param name="source">The collection source.</param>
        /// <returns>A random object or null if the list is empty</returns>
        public static T NextRandom<T>(this List<T> source)
        {
            if (source.isNull() || source.Count() < 1)
                return default(T);
            if (mRandom == null)
            {
                mRandom = new Random((int)DateTime.UtcNow.Ticks);
            }
            return source.Skip(mRandom.Next(0, source.Count())).FirstOrDefault();
        }
    }

    /// <summary>
    /// Extension methods for String
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines if a string is empty or null
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>If the string is empty or null</returns>
        public static bool isEmpty(this String s)
        {
            return String.IsNullOrEmpty(s) || ( s == "\0" );
        }

        /// <summary>
        /// Determines if a string is not empty or null
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>If the string is not empty or null</returns>
        public static bool isNotEmpty(this String s)
        {
            return !s.isNull() && !s.isEmpty();
        }

        /// <summary>
        /// Appends a directory separator character on the end of the string if one isn't already present.
        /// </summary>
        /// <param name="str">The string to modify</param>
        /// <returns>The new string</returns>
        public static String endDir(this string str)
        {
            if (str == null)
                return null;
            String directorySeparator = System.IO.Path.DirectorySeparatorChar.ToString();
            if (!str.EndsWith(directorySeparator))
                str += directorySeparator;
            return str;
        }

        /// <summary>
        /// Gets the portion of the string that are the same from the beginning.
        /// </summary>
        /// <param name="s">The main string</param>
        /// <param name="otherString">The string to compare to</param>
        /// <returns>The portion the two strings have in common.</returns>
        public static String MatchFromStart(this String s, String otherString)
        {
            if (s.isEmpty() || otherString.isEmpty())
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            int index = 0;
            while (index < s.Length && index < otherString.Length && s[index] == otherString[index])
            {
                sb.Append((s[index]));
                index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the portion of the string that are the same from the end.
        /// </summary>
        /// <param name="s">The main string</param>
        /// <param name="otherString">The string to compare to</param>
        /// <returns>The portion the two strings have in common.</returns>
        public static String MatchFromEnd(this String s, String otherString)
        {
            if (s.isEmpty() || otherString.isEmpty())
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            int index = s.Length - 1;
            int otherIndex = otherString.Length - 1;
            while (index > 0 && otherIndex > 0 && s[index] == otherString[otherIndex])
            {
                sb.Insert(0, s[index]);
                index--;
                otherIndex--;
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// List extension methods.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Determines if the list is null or empty.
        /// </summary>
        /// <param name="list">The list to check.</param>
        /// <returns>If the list is null or empty</returns>
        public static bool isEmpty(this List<String> list)
        {
            if (list == null)
                return true;
            if (list.Count < 1)
                return true;
            foreach (String item in list)
            {
                if (!item.isEmpty())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Turns a list of Strings into one string.
        /// </summary>
        /// <param name="list">The list to opearate on.</param>
        /// <param name="separator">The separator string to insert between the list elements.</param>
        /// <returns>The new string.</returns>
        public static String Stringify(this List<String> list, String separator)
        {
            if (list.isEmpty() || list.Count < 1)
                return String.Empty;
            if (separator.isNull())
            {
                separator = String.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (String item in list)
            {
                if (!item.isEmpty())
                {
                    sb.Append(item);
                    sb.Append(separator);
                }
            }
            String resultString = sb.ToString();
            return resultString.Substring(0, resultString.Length - separator.Length);
        }
    }
    
    /// <summary>
    /// Int64 extension methods
    /// </summary>    
    public static class Int64Extensions
    {
        /// <summary>
        /// Parses a string containing a hexadecimal number and returns an integer
        /// </summary>
        /// <param name="list">The list to opearate on.</param>
        /// <param name="separator">The separator string to insert between the list elements.</param>
        /// <returns>The new string.</returns>    
        public static Int64 ParseHex(this Int64 integer, String hexValue)
        {
            if (hexValue.ToUpper().StartsWith("0X"))
            {
                hexValue = hexValue.Substring(2);
            }
            integer = 0;
            try
            {
                integer = Int64.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception)
            { }
            return integer;
        }    
    }
    
    public static class TypeExtension
    {
        public static bool inherits(this Type childClass, Type parentClass)
        {
            while (parentClass != typeof(object))
            {
                var cur = parentClass.IsGenericType ? parentClass.GetGenericTypeDefinition() : parentClass;
                if (childClass == cur)
                {
                    return true;
                }
                parentClass = parentClass.BaseType;
            }
            return false;
        }
    }    
}
