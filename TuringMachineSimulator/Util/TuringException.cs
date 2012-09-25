using System;

namespace TuringMachineSimulator.Util
{
    public class TuringException : Exception
    {
        public TuringException() : base() { }
        public TuringException(string message) : base(message) { }
        public TuringException(string message, Exception innerException) : base(message, innerException) { }
    }
}
