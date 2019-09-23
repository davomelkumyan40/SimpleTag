using FrameTag.StgServices;
using Stglib.StgServices;
using System;
using System.IO;

namespace Stglib.Stg
{
    public static class StgSerializer
    {
        public static void Serialize(object obj, Stream stream)
        {
            Serialize(obj, stream, WritingFormat.Indented);
        }

        public static void Serialize(object obj, Stream stream, WritingFormat writingFormat = WritingFormat.InOneLine)
        {
            Type t = obj.GetType();
            using (StgWriter writer = new StgTextWriter(stream, writingFormat))
            {
                writer.WriteStgStart();
                if (!t.IsValueType && t != typeof(string) && !t.IsArray)
                {
                    writer.WriteObjectStart(t.Name);
                    foreach (var item in t.GetProperties())
                    {
                        if (item.PropertyType.IsArray)
                            writer.WriteArray(item.Name, (Array)item.GetValue(obj));
                        else if (item.PropertyType == typeof(string))
                            writer.WriteString(item.Name, (string)item.GetValue(obj));
                        else
                            writer.WriteValueType(item.Name, (ValueType)item.GetValue(obj));
                    }
                    foreach (var item in t.GetFields())
                    {
                        if (item.FieldType.IsArray)
                            writer.WriteArray(item.Name, (Array)item.GetValue(obj));
                        else if (item.FieldType == typeof(string))
                            writer.WriteString(item.Name, (string)item.GetValue(obj));
                        else
                            writer.WriteValueType(item.Name, (ValueType)item.GetValue(obj));
                    }
                    writer.WriteObjectEnd();

                }
                else if (t.IsValueType)
                    writer.WriteValueType(nameof(obj), (ValueType)obj);
                else if (t.IsArray)
                    writer.WriteArray(nameof(obj), (Array)obj);
                else
                    writer.WriteString(nameof(obj), (string)obj);
                writer.WriteStgEnd();
            }
        }
    }
}
