using Stglib.StgServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Stglib.Stg
{
    public static class StgDeserializer
    {
        public static SObject Deserialize(string stgScript)
        {
            SObjectInfo result = new SObjectInfo();

            List<string> script = stgScript.SplitStgScript(' ');

            while (script.Count != 0)
            {
                switch (script[0])
                {
                    case StgFormat.OBJECT_START:
                        {
                            string objTypeName = script.Skip(1).Take(1).Single();
                            result = GetMembers(objTypeName, script.ToArray());
                            result.Name = objTypeName;
                            script.RemoveRange(0, script.Count(c => c != StgFormat.OBJECT_TAG_END));
                        }
                        break;
                    case StgFormat.STRING_START:
                        {
                            string value = new string(script.Skip(3).Take(1).Single().
                            Where(c => c != char.Parse(StgFormat.STRING_FORMAT) || c != char.Parse(StgFormat.FIELD_END))
                            .ToArray());
                            result.Name = script.Skip(1).Take(1).Single();
                            var d = new KeyValuePair<Type, object>(typeof(string), value.Replace(StgFormat.STRING_FORMAT, string.Empty));
                            result.Fields.Add(script.Skip(1).Take(1).Single(), d);
                            script.RemoveRange(0, 4);
                        }
                        break;
                    case StgFormat.VARIABLES_START:
                        {
                            string value = new string(script.Skip(3).Take(1).Single()
                                .Where(c => c != char.Parse(StgFormat.FIELD_END)).ToArray()).Replace(StgFormat.STRING_FORMAT, string.Empty);
                            TryGetTypeAndFormat(value, out Type type, out string format);
                            result.Name = script.Skip(1).Take(1).Single();
                            if (format != null)
                                value = value.Replace(format, string.Empty);
                            if (type != null)
                            {
                                var d = new KeyValuePair<Type, object>(type, value.CastToValueType(type));
                                result.Fields.Add(script.Skip(1).Take(1).Single(), d);
                            }
                            script.RemoveRange(0, 3);
                        }
                        break;
                    case StgFormat.ARRAY_START:
                        {
                            string arrayValues = new string(script.Skip(4).TakeWhile(c => !c.Contains(StgFormat.ARRAY_TAG_END))
                                  .ToArray().Aggregate((res, item) => res + item)
                                  .Where(c => c != char.Parse(StgFormat.ARRAY_TAG_START) && c != char.Parse(StgFormat.ARRAY_TAG_END)).ToArray());
                            string oneValue = new string(arrayValues.TakeWhile(c => c != ',').ToArray()).Replace(StgFormat.STRING_FORMAT, string.Empty);
                            TryGetTypeAndFormat(oneValue, out Type t, out string format, true);
                            result.Name = t.Name;
                            if (format != null)
                                arrayValues = arrayValues.Replace(format, string.Empty);
                            if (t != null)
                            {
                                var d = new KeyValuePair<Type, object>(t, arrayValues.CastToArray(t));
                                result.Fields.Add(script.Skip(1).Take(1).Single(), d);
                            }
                            script = script.SkipWhile(c => !c.Contains(StgFormat.ARRAY_TAG_END)).Skip(1).ToList();
                        }
                        break;
                    case StgFormat.COMMENT_FORMAT_START:
                        {
                            script = script.SkipWhile(c => c != StgFormat.COMMENT_FORMAT_END).Skip(1).ToList();
                        }
                        break;
                    default:
                        script.RemoveAt(0);
                        break;
                }
            }
            foreach (var item in result.Fields)
            {
                result.AddMember(item.Value.Key, item.Value.Value);
            }
            return result as SObject;
        }

        private static SObjectInfo GetMembers(string objName, string[] script)
        {
            SObjectInfo objRes = new SObjectInfo();
            var objBody = script.SkipWhile(s => s != objName).Skip(2)
                .Take(script.SkipWhile(s => s != objName).Skip(2)
                .Count(s => s != StgFormat.OBJECT_TAG_END) + 1).ToList();

            while (objBody.Count != 0)
            {
                switch (objBody[0])
                {
                    case StgFormat.STRING_START:
                        {
                            var d = new KeyValuePair<Type, object>(typeof(string), new string(objBody.Skip(3).Take(1).Single().ToArray()).Replace(StgFormat.STRING_FORMAT, ""));
                            objRes.Name = objBody.Skip(1).Take(1).Single();
                            objRes.Fields.Add(objBody.Skip(1).Take(1).Single(), d);
                            objBody.RemoveRange(0, 5);
                        }
                        break;
                    case StgFormat.VARIABLES_START:
                        {
                            string value = new string(objBody.Skip(3).Take(1).Single()
                                .Where(c => c != char.Parse(StgFormat.FIELD_END)).ToArray()).Replace(StgFormat.STRING_FORMAT, string.Empty);
                            TryGetTypeAndFormat(value, out Type type, out string format);
                            objRes.Name = objBody.Skip(1).Take(1).Single();
                            if (format != null)
                                value = value.Replace(format, string.Empty);
                            if (type != null)
                            {
                                var d = new KeyValuePair<Type, object>(type, value.CastToValueType(type));
                                objRes.Fields.Add(objBody.Skip(1).Take(1).Single(), d);
                            }
                            objBody.RemoveRange(0, 3);
                        }
                        break;
                    case StgFormat.ARRAY_START:
                        {
                            string arrayValues = new string(objBody.Skip(4).TakeWhile(c => !c.Contains(StgFormat.ARRAY_TAG_END))
                                  .ToArray().Aggregate((res, item) => res + item)
                                  .Where(c => c != char.Parse(StgFormat.ARRAY_TAG_START) && c != char.Parse(StgFormat.ARRAY_TAG_END)).ToArray());
                            string oneValue = new string(arrayValues.TakeWhile(c => c != ',').ToArray()).Replace(StgFormat.STRING_FORMAT, string.Empty);
                            TryGetTypeAndFormat(oneValue, out Type t, out string format, true);
                            if (format != null)
                                arrayValues = arrayValues.Replace(format, string.Empty);
                            if (t != null)
                            {
                                var d = new KeyValuePair<Type, object>(t, arrayValues.CastToArray(t));
                                objRes.Fields.Add(objBody.Skip(1).Take(1).Single(), d);
                            }
                            objBody = objBody.SkipWhile(c => !c.Contains(StgFormat.ARRAY_TAG_END)).Skip(1).ToList();
                        }
                        break;
                    case StgFormat.COMMENT_FORMAT_START:
                        {
                            objBody = objBody.SkipWhile(c => c != StgFormat.COMMENT_FORMAT_END).Skip(1).ToList();
                        }
                        break;
                    default:
                        objBody.RemoveAt(0);
                        break;
                }
            }
            return objRes;
        }


        private static void TryGetTypeAndFormat(string value, out Type type, out string format, bool isArray = false)
        {
            format = default;
            type = null;
            char @char;
            int @int;
            double @double;
            long @long;
            bool @bool;
            DateTime dateTime;

            bool isNum = false;
            for (int i = 0; i < value.Length; i++)
                if (!char.IsNumber(value[i]) && value[i] != '.' || value.Contains(":"))
                {
                    isNum = false;
                    break;
                }
                else
                    isNum = true;
            if (isNum)
            {
                if (value.Contains("."))
                {
                    if (double.TryParse(value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out @double))
                    {
                        if (isArray)
                            type = typeof(double[]);
                        else
                            type = typeof(double);
                        return;
                    }
                }
                if (int.TryParse(value, out @int))
                {
                    if (isArray)
                        type = typeof(int[]);
                    else
                        type = typeof(int);
                    return;
                }
                if (long.TryParse(value, out @long))
                {
                    if (isArray)
                        type = typeof(long[]);
                    else
                        type = typeof(long);
                    return;
                }

            }
            if (value.StartsWith(StgFormat.CHAR_FORMAT))
            {
                value = value.Replace(StgFormat.CHAR_FORMAT, string.Empty);
                if (char.TryParse(value, out @char))
                {
                    if (isArray)
                        type = typeof(char[]);
                    else
                        type = typeof(char);
                    format = StgFormat.CHAR_FORMAT;
                    return;
                }
            }
            if (value.ToLower() == "true" || value.ToLower() == "false")
            {
                bool.TryParse(value, out @bool);
                if (isArray)
                    type = typeof(bool[]);
                else
                    type = typeof(bool);
                return;
            }
            if (value.Contains(":"))
            {
                if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime))
                {
                    if (isArray)
                        type = typeof(DateTime[]);
                    else
                        type = typeof(DateTime);
                    format = StgFormat.STRING_FORMAT;
                    return;
                }
            }
            if (isArray)
                type = typeof(string[]);
            else
                type = typeof(string);
            format = StgFormat.STRING_FORMAT;
        }
    }
}
