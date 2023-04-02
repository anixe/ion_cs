using System;
using System.IO;

namespace Anixe.Ion
{
    public static class IonWriterFactory
    {
        /// <summary>
        /// Creates the instance of <see cref="IIonReader"/> for specified file path.
        /// It opens file with <see cref="FileMode.Open"/>, <see cref="FileAccess.Read"/>
        /// and <see cref="FileShare.ReadWrite"/>.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <exception cref="ArgumentException">Undefined file path</exception>
        /// <exception cref="ArgumentException">Not existing file</exception>
        public static IIonWriter Create(string filePath)
        {
            ValidateFilePath(filePath);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

            return Create(fileStream);
        }

        /// <summary>
        /// Creates the instance of IIonWriter from specified stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="leaveOpen">Indicates if the stream will be disposed with IIonWriter.Dispose</param>
        public static IIonWriter Create(Stream stream, bool leaveOpen = false)
        {
            var writer = new StreamWriter(stream);
            return Create(writer, new WriterOptions { LeaveOpen = leaveOpen });
        }

        /// <summary>
        /// Creates the instance of <see cref="IIonWriter"/> from specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="tw"><see cref="TextWriter"/> used to write.</param>
        /// <param name="leaveOpen">Indicates if the <see cref="TextWriter"/> instance will be disposed with IIonWriter.Dispose</param>
        public static IIonWriter Create(TextWriter tw, bool leaveOpen = false)
        {
            return new IonWriter(tw, new WriterOptions { LeaveOpen = leaveOpen });
        }

        /// <summary>
        /// Creates the instance of <see cref="IIonWriter"/> from specified stream.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> to write to.</param>
        public static IIonWriter Create(Stream stream, WriterOptions options)
        {
            var writer = new StreamWriter(stream);
            return Create(writer, options);
        }

        /// <summary>
        /// Creates the instance of <see cref="IIonWriter"/> from specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="tw"><see cref="TextWriter"/> used to write.</param>
        /// <param name="options">Writing options.</param>
        public static IIonWriter Create(TextWriter tw, WriterOptions options)
        {
            return new IonWriter(tw, options);
        }

        #region Private methods

        private static void ValidateFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(Consts.ErrorMessages.UndefinedFilePath);
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException(string.Format(Consts.ErrorMessages.FileDoesNotExist, filePath));
            }
        }

        #endregion
    }
}
