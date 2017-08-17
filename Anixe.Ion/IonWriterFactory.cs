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
        /// Creates the instance of IIonReader for specified stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public static IIonWriter Create(Stream stream)
        {
            var writer = new StreamWriter(stream);
            return Create(writer);
        }

        public static IIonWriter Create(TextWriter tw)
        {
            return new IonWriter(tw);
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

