using System;
using System.IO;

namespace Stglib.StgServices
{
    public class StgTextReader : StgReader
    {
        //ctor
        public StgTextReader(Stream stgStream)
        {
            if (stgStream != null)
                InnerReader = new StreamReader(stgStream);
            else
                throw new NullReferenceException();
        }

        //ctor
        public StgTextReader(StreamReader stgReader)
        {
            if (stgReader != null)
                InnerReader = stgReader;
            else
                throw new NullReferenceException();
        }

        protected override StreamReader InnerReader { get; set; }

        public override string ReadAllScript()
        {
            if (InnerReader != null)
            {
                return InnerReader.ReadToEnd();
            }
            else
                throw new NullReferenceException();
        }
    }
}
