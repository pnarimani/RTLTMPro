using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace RTLTMPro
{
    public class FastStringBuilder
    {
        // Using fields to be as efficient as possible
        public int Length;

        private char[] array;
        private int capacity;

        public FastStringBuilder(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            this.capacity = capacity;
            array = new char[capacity];
        }

        public FastStringBuilder(string text) : this(text, text.Length)
        {
        }

        public FastStringBuilder(string text, int capacity) : this(capacity)
        {
            SetValue(text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Get(int index) => array[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, char ch) => array[index] = ch;

        public void SetValue(string text)
        {
            Length = text.Length;
            EnsureCapacity(Length, false);

            for (var i = 0; i < text.Length; i++) array[i] = text[i];
        }

        public void SetValue(FastStringBuilder other)
        {
            EnsureCapacity(other.Length, false);
            Copy(other.array, array);
            Length = other.Length;
        }

        public void Append(char ch)
        {
            if (capacity < Length) EnsureCapacity(Length, true);

            array[Length] = ch;
            Length++;
        }

        public void Insert(int pos, FastStringBuilder str, int offset, int count)
        {
            if (str == this) throw new InvalidOperationException("You cannot pass the same string builder to insert");
            if (count == 0) return;

            Length += count;
            EnsureCapacity(Length, true);

            for (int i = Length - count - 1; i >= pos; i--)
            {
                array[i + count] = array[i];
            }

            for (int i = 0; i < count; i++)
            {
                array[pos + i] = str.array[offset + i];
            }
        }

        public void Insert(int pos, FastStringBuilder str)
        {
            Insert(pos, str, 0, str.Length);
        }

        public void Insert(int pos, char ch)
        {
            Length++;
            EnsureCapacity(Length, true);

            for (var i = Length - 2; i >= pos; i--)
                array[i + 1] = array[i];

            array[pos] = ch;
        }

        public void RemoveAll(char character)
        {
            for (var i = Length - 1; i >= 0; i--)
                if (array[i] == character)
                    Remove(i, 1);
        }

        public void Remove(int start, int length)
        {
            for (var i = start; i < Length - length; i++)
            {
                array[i] = array[i + length];
            }

            Length -= length;
        }

        public void Reverse(int startIndex, int length)
        {
            for (var i = 0; i < length / 2; i++)
            {
                var firstIndex = startIndex + i;
                var secondIndex = startIndex + length - i - 1;

                var first = array[firstIndex];
                var second = array[secondIndex];

                array[firstIndex] = second;
                array[secondIndex] = first;
            }
        }

        public void Reverse()
        {
            Reverse(0, Length);
        }

        public void Substring(FastStringBuilder output, int start, int length)
        {
            output.Length = 0;
            for (var i = 0; i < length; i++) output.Append(array[start + i]);
        }

        public override string ToString()
        {
            return new string(array, 0, Length);
        }

        public void Replace(char oldChar, char newChar)
        {
            for (int i = 0; i < Length; i++)
            {
                if (array[i] == oldChar)
                    array[i] = newChar;
            }
        }

        public void Replace(string oldStr, string newStr)
        {
            for (int i = 0; i < Length; i++)
            {
                bool match = true;
                for (int j = 0; j < oldStr.Length; j++)
                {
                    if (array[i + j] != oldStr[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (!match) continue;

                if (oldStr.Length == newStr.Length)
                {
                    for (int k = 0; k < oldStr.Length; k++)
                    {
                        array[i + k] = newStr[k];
                    }
                }
                else if (oldStr.Length < newStr.Length)
                {
                    // We need to expand capacity
                    var diff = newStr.Length - oldStr.Length;
                    Length += diff;
                    EnsureCapacity(Length, true);

                    // Move everything forward by difference of length
                    for (int k = Length - diff - 1; k >= i + oldStr.Length; k--)
                    {
                        array[k + diff] = array[k];
                    }

                    // Start writing new string
                    for (int k = 0; k < newStr.Length; k++)
                    {
                        array[i + k] = newStr[k];
                    }
                }
                else
                {
                    // We need to shrink
                    var diff = oldStr.Length - newStr.Length;

                    // Move everything backwards by diff
                    for (int k = i + diff; k < Length - diff; k++)
                    {
                        array[k] = array[k + diff];
                    }

                    for (int k = 0; k < newStr.Length; k++)
                    {
                        array[i + k] = newStr[k];
                    }

                    Length -= diff;
                }

                i += newStr.Length;
            }
        }

        public void Clear()
        {
            Length = 0;
        }

        private void EnsureCapacity(int cap, bool keepValues)
        {
            if (capacity >= cap)
                return;

            if (capacity == 0)
                capacity = 1;

            while (capacity < cap)
                capacity *= 2;
            
            if (keepValues)
            {
                var newArray = new char[capacity];
                Copy(array, newArray);
                array = newArray;
            }
            else
            {
                array = new char[capacity];
            }
        }

        private void Copy(char[] src, char[] dst)
        {
            for (var i = 0; i < src.Length; i++) dst[i] = src[i];
        }
    }
}