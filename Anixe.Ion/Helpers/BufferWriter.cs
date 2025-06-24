using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Anixe.Ion.Helpers
{
    internal struct BufferWriter : IDisposable
    {
        private readonly ArrayPool<char> pool;
        private char[]? buffer;

        public int Count { get; private set; }
        public readonly ArraySegment<char> WrittenSegment => this.Count == 0
            ? ArraySegment<char>.Empty
            : new ArraySegment<char>(this.buffer ?? [], 0, this.Count);

        public BufferWriter(ArrayPool<char> pool)
        {
          this.buffer = null;
          this.Count = 0;
          this.pool = pool;
        }

        public void Write(char[] value, int startIndex, int charCount)
        {
          EnsureCapacity(this.Count + charCount);
          Array.Copy(value, startIndex, this.buffer, this.Count, charCount);
          this.Count += charCount;
        }

        [MemberNotNull(nameof(this.buffer))]
        private void EnsureCapacity(int length)
        {
            if (this.buffer != null)
            {
                if (this.buffer.Length >= length)
                {
                    return;
                }
                var newBuffer = this.pool.Rent(length);
                Array.Copy(this.buffer, 0, newBuffer, 0, this.Count);
                this.pool.Return(this.buffer);
                this.buffer = newBuffer;
            }
            else
            {
                this.buffer = this.pool.Rent(length);
            }
        }

        public void Clear()
        {
          this.Count = 0;
        }

        public void Dispose()
        {
            if (this.buffer != null)
            {
                this.pool.Return(this.buffer);
            }
        }
    }
}
