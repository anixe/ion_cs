using System;
using System.Buffers;
using System.IO;

namespace Anixe.Ion
{
    public static class IonReaderFactory
    {
        /// <summary>
        /// Creates the instance of IIonReader for specified file path. It opens file with FileMode.Open, FileAccess.Read and FileShare.ReadWrite and closes underlying stream in Dispose
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <exception cref="ArgumentException">Undefined file path</exception>
        /// <exception cref="ArgumentException">Not existing file</exception>
        public static IIonReader Create(string filePath)
        {
            ValidateFilePath(filePath);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return Create(fileStream);
        }

        /// <summary>
        /// Creates the instance of IIonReader for specified stream. It closes underlying stream in Dispose
        /// </summary>
        /// <param name="stream">Stream.</param>
        public static IIonReader Create(Stream stream)
        {
            return Create(stream, false);
        }

        /// <summary>
        /// Creates the instance of IIonReader for specified stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="leaveOpen">If True: it closes underlying stream in Dispose. If False: The stream will not be disposed.</param>
        /// <param name="charPool">Provide own System.Buffers.ArrayPool<char> instance. If null then System.Buffers.ArrayPool<char>.Shared will be used</param>
        public static IIonReader Create(Stream stream, bool leaveOpen = false, ArrayPool<char>? charPool = null)
        {
            return new IonReader(stream, new CurrentLineVerifier(), new SectionHeaderReader(), leaveOpen, charPool);
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

