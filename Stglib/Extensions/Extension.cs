using Stglib.StgServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Globalization;
using Stglib.Exceptions;

namespace Stglib.Stg
{
    public static class Extensions
    {
        public static List<string> SplitStgScript(this string str, char by)
        {
            if (!str.StartsWith(StgFormat.STG_START) && !str.EndsWith(StgFormat.STG_END))
                throw new WrongStgContextException();
            if (str.Contains("\r\n"))
                str = str.Replace("\r\n", " ");
            if (str.Contains(StgFormat.COMMENT_FORMAT_START))
                str = str.Replace(StgFormat.COMMENT_FORMAT_START, $" {StgFormat.COMMENT_FORMAT_START} ");
            if (str.Contains(StgFormat.COMMENT_FORMAT_END))
                str = str.Replace(StgFormat.COMMENT_FORMAT_END, $" {StgFormat.COMMENT_FORMAT_END} ");

            string temp = string.Empty;
            List<string> list = new List<string>();
            bool string_start = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (string_start)
                {
                    if (str[i] == char.Parse(StgFormat.STRING_FORMAT))
                    {
                        temp += str[i];
                        list.Add(temp);
                        temp = string.Empty;
                        string_start = false;
                    }
                    else
                        temp += str[i];
                }
                else if (str[i] == char.Parse(StgFormat.STRING_FORMAT))
                {
                    string_start = true;
                    temp = string.Empty;
                    temp += str[i];
                }
                else if (char.IsWhiteSpace(str[i]))
                {
                    list.Add(temp);
                    temp = string.Empty;
                }
                else
                    temp += str[i];
            }
            list.Add(temp);
            return list.Where(c => c != string.Empty).ToList();
        }

        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static ValueType CastToValueType(this string value, Type type)
        {
            ValueType v = null;
            if (type == typeof(int))
                v = int.Parse(value);
            if (type == typeof(double))
                v = double.Parse(value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"));
            if (type == typeof(long))
                v = long.Parse(value);
            if (type == typeof(DateTime))
                v = DateTime.Parse(value, CultureInfo.CurrentCulture);
            if (type == typeof(char))
                v = char.Parse(value);
            return v;
        }

        public static Array CastToArray(this string values, Type type)
        {
            Array obj = null;
            if (type.GetElementType() == typeof(int))
                obj = values.Split(',').Where(c => !string.IsNullOrWhiteSpace(c) && c != string.Empty).Select(c => int.Parse(c)).ToArray();
            else if (type.GetElementType() == typeof(double))
                obj = values.Split(',').Where(c => !string.IsNullOrWhiteSpace(c) && c != string.Empty).Select(c => double.Parse(c, CultureInfo.CreateSpecificCulture("en-US"))).ToArray();
            else if (type.GetElementType() == typeof(long))
                obj = values.Split(',').Where(c => !string.IsNullOrWhiteSpace(c) && c != string.Empty).Select(c => long.Parse(c)).ToArray();
            else if (type.GetElementType() == typeof(string))
                obj = values.Split(',').Where(c => !string.IsNullOrWhiteSpace(c) && c != string.Empty).ToArray();
            else if (type.GetElementType() == typeof(char))
                obj = values.Split(',').Where(c => !string.IsNullOrWhiteSpace(c) && c != string.Empty).Select(c => char.Parse(c)).ToArray();
            else if (type.GetElementType() == typeof(DateTime))
                obj = values.Split(',').Where(c => !string.IsNullOrWhiteSpace(c) && c != string.Empty).Select(c => DateTime.Parse(c, CultureInfo.CurrentCulture)).ToArray();
            else if (type.GetElementType() == typeof(bool))
                obj = values.Split(',').Where(c => c != "" && c != " ").Select(c => bool.Parse(c)).ToArray();
            return obj;
        }
    }
}
