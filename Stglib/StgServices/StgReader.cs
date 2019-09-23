using System;
using System.IO;


namespace Stglib.StgServices
{
    public abstract class StgReader : IDisposable
    {
        protected abstract StreamReader InnerReader { get; set; }

        public abstract string ReadAllScript();

        public void Dispose()
        {
            InnerReader.Dispose();
        }
    }
}
