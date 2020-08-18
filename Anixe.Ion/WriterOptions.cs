namespace Anixe.Ion
{
    public class WriterOptions
    {
        /// <summary>
        /// Set to true to leave provided stream undisposed by IonWriter's Dispose method.
        /// </summary>
        public bool LeaveOpen { get; set; }

        /// <summary>
        /// Use this option if IonWriter serializes ion content as a part of other document in different format.null Eg. ion formatted value is a part of json's property value.
        /// </summary>
        public bool EscapeQuotes { get; set; }
    }
}