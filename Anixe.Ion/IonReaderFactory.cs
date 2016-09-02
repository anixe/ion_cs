using System;
using System.IO;

namespace Anixe.Ion
{
    public static class IonReaderFactory
    {
        /// <summary>
        /// Create instance of IIonReader for specified file path. It opens file with FileMode.Open, FileAccess.Read and FileShare.ReadWrite
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <exception cref="ArgumentException">Undefined file path</exception>
        /// <exception cref="ArgumentException">Not existing file</exception>
        public static IIonReader Create(string filePath)
        {
            ValidateFilePath(filePath);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return Create(fileStream);
        }

        /// <summary>
        /// Create instance of IIonReader for specified stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public static IIonReader Create(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);

            return new IonReader(streamReader, new CurrentLineVerifier(), new SectionHeaderReader());
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

