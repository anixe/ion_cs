namespace Anixe.Ion
{
    public class WriterOptions
    {
        /// <summary>
        /// Set to <see langword="true"/> to leave provided stream undisposed by IonWriter's Dispose method.
        /// </summary>
        public bool LeaveOpen { get; set; }

        /// <summary>
        /// Use this option if <see cref="IIonWriter"/> serializes ion content as a part of other document in different format.
        /// E.g. ion formatted value is a part of json's property value.
        /// </summary>
        public bool EscapeQuotes { get; set; }

        /// <summary>
        /// Use this option if <see cref="IIonWriter"/> serializes ion content as a part of other document in different format.
        /// E.g. ion formatted value is a part of json's property value.
        /// </summary>
        public bool EscapeNewLineChars { get; set; }
    }
}