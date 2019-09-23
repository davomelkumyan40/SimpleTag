using System;
using System.IO;
using FrameTag.StgServices;
using Stglib.StgServices;

namespace Stglib.Stg
{
    public static class StgConvert
    {
        public static void SerializeObject(object obj, Stream stream)
        {
            if (stream != null)
                StgSerializer.Serialize(obj, stream);
            else
                throw new NullReferenceException();
        }

        public static void SerializeObject(object obj, Stream stream, WritingFormat writingFormat)
        {
            if (stream != null)
                StgSerializer.Serialize(obj, stream, writingFormat);
            else
                throw new NullReferenceException();
        }

        public static SObject DeserializeObject(string stgScript)
        {
            return StgDeserializer.Deserialize(stgScript) as SObject;
        }

        public static SObject DeserializeObject(StgReader reader)
        {
            string stgScript = reader.ReadAllScript();
            return StgDeserializer.Deserialize(stgScript) as SObject;
        }

        public static T DeserializeObject<T>(string stgScript)
        {
            T instance = Activator.CreateInstance<T>();
            SObject object_ = StgDeserializer.Deserialize(stgScript);
            return object_.Cast<T>();
        }

        public static object DeserializeObject(string stgScript, Type t)
        {
            object instance = Activator.CreateInstance(t);
            SObject object_ = StgDeserializer.Deserialize(stgScript);
            return object_.Cast(t);
        }
    }
}
