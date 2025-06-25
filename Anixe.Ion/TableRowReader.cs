using System;
using System.Buffers;

namespace Anixe.Ion;

/// <summary>
/// Provides functionality to parse and read individual cell values from a table row represented as a delimited string.
/// </summary>
/// <remarks>The <see cref="TableRowReader"/> is designed to process table rows formatted with pipe ('|')
/// delimiters. It trims leading and trailing whitespace, handles escaped characters (e.g., <c>\n</c> for newlines and
/// <c>\|</c> for literal pipes), and allows sequential reading of cell values.
/// <para>See that this is ref struct</para>
/// </remarks>
public ref struct TableRowReader
{
    private static readonly SearchValues<string> EscapedCharacters = SearchValues.Create(["\\n", "\\|"], StringComparison.Ordinal);
    private ReadOnlySpan<char> span;

    internal TableRowReader(ReadOnlySpan<char> rawTableLine)
    {
        // remove trailing line endings, tabs and other whitespaces from the end
        var trimmedLine = rawTableLine.TrimEnd();

        if (!trimmedLine.StartsWith('|') || !trimmedLine.EndsWith('|'))
        {
            Throw_FormatException();
        }

        // trim pipes from start and end
        trimmedLine = trimmedLine[1..^1];

        this.span = trimmedLine.Trim([' ', '\t']);

        static void Throw_FormatException() =>
            throw new FormatException("Table row must start and end with a pipe character '|'.");
    }

#pragma warning disable CA2265 // Do not compare Span<T> to 'null' or 'default'
    /// <summary>
    /// Gets a value indicating whether there is a next cell can be read from the table row.
    /// </summary>
    public readonly bool CanReadNext => this.span != default;
#pragma warning restore CA2265

    private ReadOnlySpan<char> CurrentRawValue { get; set; }

    /// <summary>
    /// Reads the next table cell value from the current line.
    /// It unescapes escaped characters `\n` and `\|`.
    /// </summary>
    /// <returns>A string that represents value from cell.</returns>
    /// <exception cref="InvalidOperationException">No more elements to read.</exception>
    public string ReadNextString()
    {
        return ReadNextSpan().ToString();
    }

    /// <summary>
    /// Reads the next table cell value from the current line.
    /// It unescapes escaped characters `\n` and `\|`.
    /// </summary>
    /// <returns>A span that represents value from cell.</returns>
    /// <exception cref="InvalidOperationException">No more elements to read.</exception>
    public ReadOnlySpan<char> ReadNextSpan()
    {
        if (!MoveNext())
        {
            Throw_NoMoreElementsToRead();
        }

        var trimmedRawValue = this.CurrentRawValue.Trim([' ', '\t']);

        if (NeedsUnescape(trimmedRawValue))
        {
            return Unescape(trimmedRawValue);
        }

        return trimmedRawValue;

        static bool NeedsUnescape(ReadOnlySpan<char> rawValue) =>
            rawValue.ContainsAny(EscapedCharacters);

        static void Throw_NoMoreElementsToRead() =>
            throw new InvalidOperationException("No more elements to read.");
    }

    // Unescapes special sequences in a table cell value, such as '\n' to newline and '\|' to '|'
    private static ReadOnlySpan<char> Unescape(ReadOnlySpan<char> rawValue)
    {
        int len = rawValue.Length;
        Span<char> buffer = len <= 256 ? stackalloc char[len] : new char[len];
        int outIdx = 0;

        for (int i = 0; i < len;)
        {
            // Check for escape sequence starting with backslash
            if (rawValue[i] == '\\' && i + 1 < len)
            {
                char next = rawValue[i + 1];
                if (next == '|')
                {
                    // Replace '\|' with '|'
                    buffer[outIdx++] = '|';
                    i += 2;
                    continue;
                }
                else if (next == 'n')
                {
                    // Replace '\n' with newline character
                    buffer[outIdx++] = '\n';
                    i += 2;
                    continue;
                }
            }
            // Copy character as-is if not part of an escape sequence
            buffer[outIdx++] = rawValue[i++];
        }

        return buffer.Slice(0, outIdx).ToString();
    }

    private bool MoveNext()
    {
        if (this.span.IsEmpty)
        {
            if (!this.CanReadNext)
            {
                return false;
            }

            this.CurrentRawValue = this.span;
            this.span = default;
            return true;
        }

        // Hot path: no escaping present, just split on '|'
        int delimiterIdx = this.span.IndexOf('|');
        if (delimiterIdx >= 0 && !HasAnyBackslash(this.span.Slice(0, delimiterIdx)))
        {
            this.CurrentRawValue = this.span.Slice(0, delimiterIdx);
            this.span = this.span.Slice(delimiterIdx + 1);
            return true;
        }

        if (delimiterIdx == -1 && !HasAnyBackslash(this.span))
        {
            this.CurrentRawValue = this.span;
            this.span = default;
            return true;
        }

        // Fallback: slow path with escape handling
        ExtractEscapedSegment();
        return true;

        static bool HasAnyBackslash(ReadOnlySpan<char> span)
        {
            return span.IndexOf('\\') >= 0;
        }
    }

    private void ExtractEscapedSegment()
    {
        int delimiterIdx = -1; // Index of the next unescaped pipe delimiter
        int i = 0;
        // Iterate through the span to find the next unescaped '|'
        while (i < this.span.Length)
        {
            // Find the next '|' character from the current position
            int nextIdx = this.span.Slice(i).IndexOf('|');
            if (nextIdx == -1)
            {
                // No more '|' found, break out of the loop
                break;
            }
            int candidateIdx = i + nextIdx;
            int backslashCount = 0;
            int j = candidateIdx - 1;
            // Count consecutive backslashes before the '|' to determine if it's escaped
            while (j >= 0 && this.span[j] == '\\')
            {
                backslashCount++;
                j--;
            }
            // If the number of backslashes is even, the '|' is not escaped
            if (backslashCount % 2 == 0)
            {
                delimiterIdx = candidateIdx;
                break;
            }
            // Move past the escaped '|' and continue searching
            i = candidateIdx + 1;
        }

        if (delimiterIdx < 0)
        {
            // No unescaped '|' found, take the rest of the span as the value
            this.CurrentRawValue = this.span;
            this.span = default;
        }
        else
        {
            // Extract the segment up to the unescaped '|' and advance the span
            this.CurrentRawValue = this.span.Slice(0, delimiterIdx);
            this.span = this.span.Slice(delimiterIdx + 1);
        }
    }
}
