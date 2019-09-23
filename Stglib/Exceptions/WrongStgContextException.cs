using System;

namespace Stglib.Exceptions
{
    class WrongStgContextException : Exception
    {
        public override string Message => "Wrong Context in STG file. Uncorrect script";
    }
}
