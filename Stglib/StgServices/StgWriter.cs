using FrameTag.StgServices;
using System;
using System.IO;

namespace Stglib.StgServices
{
    public abstract class StgWriter : IDisposable
    {
        protected abstract StreamWriter InnerWriter { get; set; }
        protected abstract WritingFormat WritingFormat { get; set; }

        public abstract void WriteComment(string comment);
        public abstract void WriteString(string name, string value);
        public abstract void WriteValueType(string name, ValueType value);
        public abstract void WriteStgStart();
        public abstract void WriteStgEnd();
        public abstract void WriteArray(string name, Array values);
        public abstract void WriteObjectStart(string name);
        public abstract void WriteObjectEnd();

        public void Dispose()
        {
            InnerWriter.Dispose();
        }
    }
}
