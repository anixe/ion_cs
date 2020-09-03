using System;

namespace Anixe.Ion.Exceptions
{
    public class IonWriterException : Exception
    {
        public IonWriterException() : base()
        {
        }

        public IonWriterException(string message) : base(message)
        {
        }

        public IonWriterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
