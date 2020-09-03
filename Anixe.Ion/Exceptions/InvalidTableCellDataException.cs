namespace Anixe.Ion.Exceptions
{
    public class InvalidTableCellDataException : IonWriterException
    {
        public InvalidTableCellDataException() : base()
        {
        }

        public InvalidTableCellDataException(string message) : base(message)
        {
        }

        public InvalidTableCellDataException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
