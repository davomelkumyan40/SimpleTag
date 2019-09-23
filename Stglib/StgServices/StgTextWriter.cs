using FrameTag.StgServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Stglib.StgServices
{
    public class StgTextWriter : StgWriter
    {
        //ctor
        public StgTextWriter(Stream stream)
        {
            InnerWriter = new StreamWriter(stream);
            WritingFormat = WritingFormat.InOneLine;
        }

        //ctor
        public StgTextWriter(StreamWriter writer)
        {
            InnerWriter = writer;
            WritingFormat = WritingFormat.InOneLine;
        }

        //ctor
        public StgTextWriter(Stream stream, WritingFormat writingFormat)
        {
            InnerWriter = new StreamWriter(stream);
            WritingFormat = writingFormat;
        }

        //ctor
        public StgTextWriter(StreamWriter writer, WritingFormat writingFormat)
        {
            InnerWriter = writer;
            WritingFormat = writingFormat;
        }


        protected override StreamWriter InnerWriter { get; set; }

        protected override WritingFormat WritingFormat { get; set; }

        #region Write Functions

        public override void WriteStgStart()
        {
            if (WritingFormat == WritingFormat.Indented)
                InnerWriter.WriteLine($"{StgFormat.STG_START} ");
            else
                InnerWriter.Write($"{StgFormat.STG_START} ");
        }

        public override void WriteStgEnd()
        {
            if (WritingFormat == WritingFormat.Indented)
                InnerWriter.WriteLine(StgFormat.STG_END);
            else
                InnerWriter.Write(StgFormat.STG_END);
            Dispose();
        }

        public override void WriteObjectStart(string name)
        {
            if (WritingFormat == WritingFormat.Indented)
                InnerWriter.WriteLine($"{StgFormat.OBJECT_START} {name} {StgFormat.OBJECT_TAG_START} ");
            else
                InnerWriter.Write($"{StgFormat.OBJECT_START} {name} {StgFormat.OBJECT_TAG_START} ");
        }

        public override void WriteObjectEnd()
        {
            if (WritingFormat == WritingFormat.Indented)
                InnerWriter.WriteLine($"{StgFormat.OBJECT_TAG_END} ");
            else
                InnerWriter.Write($"{StgFormat.OBJECT_TAG_END} ");
        }

        public override void WriteComment(string comment)
        {
            if (WritingFormat == WritingFormat.Indented)
                InnerWriter.WriteLine($"{StgFormat.COMMENT_FORMAT_START} {comment} {StgFormat.COMMENT_FORMAT_END} ");
            else
                InnerWriter.Write($"{StgFormat.COMMENT_FORMAT_START} {comment} {StgFormat.COMMENT_FORMAT_END} ");
        }

        public override void WriteValueType(string name, ValueType value)
        {
            if (!HasDefaultValue(value))
            {
                string bracket = GetBracketType(value);
                if (WritingFormat == WritingFormat.Indented)
                {
                    if (value is double)
                        InnerWriter.WriteLine($"{StgFormat.VARIABLES_START} {name} {StgFormat.EQUALS_TO} {bracket}{(string)Convert.ChangeType(value, typeof(string), CultureInfo.CreateSpecificCulture("en-US"))}{bracket}{StgFormat.FIELD_END} ");
                    else
                        InnerWriter.WriteLine($"{StgFormat.VARIABLES_START} {name} {StgFormat.EQUALS_TO} {bracket}{(string)Convert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture)}{bracket}{StgFormat.FIELD_END} ");
                }
                else
                    InnerWriter.Write($"{StgFormat.VARIABLES_START} {name} {StgFormat.EQUALS_TO} {bracket}{(string)Convert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture)}{bracket}{StgFormat.FIELD_END} ");
            }
        }

        public override void WriteString(string name, string value)
        {
            if (value != null)
                if (WritingFormat == WritingFormat.Indented)
                    InnerWriter.WriteLine($"{StgFormat.STRING_START} {name} {StgFormat.EQUALS_TO} {StgFormat.STRING_FORMAT}{value}{StgFormat.STRING_FORMAT}{StgFormat.FIELD_END} ");
                else
                    InnerWriter.Write($"{StgFormat.STRING_START} {name} {StgFormat.EQUALS_TO} {StgFormat.STRING_FORMAT}{value}{StgFormat.STRING_FORMAT}{StgFormat.FIELD_END} ");
        }

        public override void WriteArray(string name, Array values)
        {
            if (values != null)
            {
                InnerWriter.Write($"{StgFormat.ARRAY_START} {name} {StgFormat.EQUALS_TO} {StgFormat.ARRAY_TAG_START} ");
                string bracket = GetBracketType(values);
                List<string> arr = new List<string>(values.Length);
                foreach (var item in values)
                {
                    if (values.GetType().GetElementType() == typeof(double))
                        arr.Add((string)Convert.ChangeType(item, typeof(string), CultureInfo.CreateSpecificCulture("en-US")));
                    else
                        arr.Add((string)Convert.ChangeType(item, typeof(string), CultureInfo.CurrentCulture));
                }
                for (int i = 0; i < arr.Count; i++)
                {
                    if (i == arr.Count - 1)
                        InnerWriter.Write($"{bracket}{arr[i]}{bracket}");
                    else
                        InnerWriter.Write($"{bracket}{arr[i]}{bracket}, ");
                }
                if (WritingFormat == WritingFormat.Indented)
                    InnerWriter.WriteLine($" {StgFormat.ARRAY_TAG_END}{StgFormat.FIELD_END} ");
                else
                    InnerWriter.Write($" {StgFormat.ARRAY_TAG_END}{StgFormat.FIELD_END} ");
            }
        }

        private string GetBracketType(Array values)
        {
            string bracket = string.Empty;
            if (values.GetType().GetElementType() == typeof(char))
                bracket = StgFormat.CHAR_FORMAT;
            if (values.GetType().GetElementType() == typeof(DateTime))
                bracket = StgFormat.STRING_FORMAT;
            if (values.GetType().GetElementType() == typeof(string))
                bracket = StgFormat.STRING_FORMAT;
            return bracket;
        }

        private string GetBracketType(ValueType value)
        {
            string bracket = string.Empty;
            if (value.GetType() == typeof(char))
                bracket = StgFormat.CHAR_FORMAT;
            if (value.GetType() == typeof(DateTime))
                bracket = StgFormat.STRING_FORMAT;
            if (value.GetType() == typeof(string))
                bracket = StgFormat.STRING_FORMAT;
            return bracket;
        }

        private bool HasDefaultValue(ValueType value)
        {
            if (value is char c && c == default(char) ||
                value is int n && n == default(int) ||
                value is long l && l == default(long) ||
                value is double f && f == default(double) ||
                value is DateTime d && d == default(DateTime))
                return true;
            else
                return false;
        }
        #endregion
    }
}
