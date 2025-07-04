﻿using Anixe.Ion.Helpers;
using System;
using System.Buffers;
using Xunit;

namespace Anixe.Ion.UnitTests.Helpers
{
    public class BufferWriterTest
    {
        [Fact]
        public void BufferWriter_Pool_Handing()
        {
            var pool = new TestPool();
            var writer = new BufferWriter(pool);
            Assert.Equal(0, writer.Count);

            writer.Write(new char[10], 0, 1);
            Assert.Equal(1, writer.Count);
            Assert.Equal(0, pool.currentBufferIndex);

            writer.Write(new char[10], 0, 2);
            Assert.Equal(3, writer.Count);
            Assert.Equal(1, pool.currentBufferIndex);

            writer.Dispose();
            //check if all buffers were returned
            Assert.True(Array.TrueForAll(pool.buffers, b => b == null));
        }

        [Fact]
        public void BufferWriter_Writing_text()
        {
            var writer = new BufferWriter(ArrayPool<char>.Shared);
            writer.Write("text1".ToCharArray(), 0, "text1".Length);
            writer.Write("something".ToCharArray(), 2, 3);
            Assert.Equal(8, writer.Count);
            Assert.NotNull(writer.WrittenSegment.Array);
            Assert.Equal("text1met", new string(writer.WrittenSegment.Array!, writer.WrittenSegment.Offset, writer.WrittenSegment.Count));
            writer.Dispose();
        }

        private class TestPool : ArrayPool<char>
        {
            public char[]?[] buffers =
            [
                new char[1], new char[3]
            ];
            public int currentBufferIndex = -1;

            public override char[] Rent(int minimumLength)
            {
                var buffer = buffers[++currentBufferIndex];
                Assert.NotNull(buffer);
                Assert.True(buffer!.Length <= minimumLength);
                return buffer;
            }

            public override void Return(char[] array, bool clearArray = false)
            {
                var idx = Array.IndexOf(this.buffers, array);
                Assert.NotNull(this.buffers[idx]);
                this.buffers[idx] = null;
                Assert.False(clearArray);
            }
        }
    }
}
