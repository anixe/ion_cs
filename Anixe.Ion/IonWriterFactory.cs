using System;
using System.IO;

namespace Anixe.Ion
{
    public static class IonWriterFactory
    {
        /// <summary>
        /// Creates the instance of IIonReader for specified file path. It opens file with FileMode.Open, FileAccess.Read and FileShare.ReadWrite
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <exception cref="ArgumentException">Undefined file path</exception>
        /// <exception cref="ArgumentException">Not existing file</exception>
        public static IIonWriter Create(string filePath)
        {
            ValidateFilePath(filePath);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

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
            return Create(writer, leaveOpen);
        }

        /// <summary>
        /// Creates the instance of IIonWriter from specified TextWriter.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="leaveOpen">Indicates if the TextWriter instance will be disposed with IIonWriter.Dispose</param>
        public static IIonWriter Create(TextWriter tw, bool leaveOpen = false)
        {
            return new IonWriter(tw, leaveOpen);
        }

        #region Private methods

        private static void ValidateFilePath(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(Consts.ErrorMessages.UndefinedFilePath);
            }

            if(!File.Exists(filePath))
            {
                throw new ArgumentException(string.Format(Consts.ErrorMessages.FileDoesNotExist, filePath));
            }
        }

        #endregion
    }
}

