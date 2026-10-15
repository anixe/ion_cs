namespace Anixe.Ion.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the data in a table cell is invalid.
    /// </summary>
    public class InvalidTableCellDataException : IonWriterException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTableCellDataException"/> class.
        /// </summary>
        public InvalidTableCellDataException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTableCellDataException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidTableCellDataException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTableCellDataException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public InvalidTableCellDataException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
