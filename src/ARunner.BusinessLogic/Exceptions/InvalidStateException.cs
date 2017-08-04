using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.BusinessLogic.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException()
        {
        }

        public InvalidStateException(string message) : base(message)
        {
        }

        public InvalidStateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidStateException<T> : InvalidStateException
    {
        public T State { get; }

        public InvalidStateException(T state)
        {
            State = state;
        }

        public InvalidStateException(T state, string message) : base(message)
        {
            State = state;
        }

        public InvalidStateException(T state, string message, Exception innerException) : base(message, innerException)
        {
            State = state;
        }
    }
}
