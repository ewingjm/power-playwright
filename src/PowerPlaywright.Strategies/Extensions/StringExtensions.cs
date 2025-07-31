namespace PowerPlaywright.Strategies.Extensions
{
    using System;

    /// <summary>
    /// Helper class to Decode a Base32 string.
    /// </summary>
    internal static class StringExtensions
    {
        private const char CAN = (char)24;
        private const int Mask = 31;
        private const int Shift = 5;

        private static int CharToInt(char c)
        {
            int value = (int)c;

            // 65-90 == uppercase letters
            if (c < '[' && c > '@')
            {
                return c - 'A';
            }

            // 50-55 == numbers 2-7
            if (value < '8' && value > '1')
            {
                return value - CAN;
            }

            // 97-122 == lowercase letters
            if (value < '{' && value > '`')
            {
                return value - 'a';
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        /// <summary>
        /// Converts a Base32 encoded string to the corresponding byte array.
        /// </summary>
        /// <param name="encoded">Encoded string to convert.</param>
        /// <returns>Byte array of the corresponding Base32 encoded string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if input is null</exception>
        /// <exception cref="FormatException">Thrown if input string is not an encoded Base32 string.</exception>
        internal static byte[] DecodeAsBase32String(this string encoded)
        {
            if (encoded == null)
            {
                throw new ArgumentNullException(nameof(encoded));
            }

            encoded = encoded.Trim().TrimEnd('=').ToUpper();
            if (encoded.Length == 0)
            {
                return Array.Empty<byte>();
            }

            var outLength = encoded.Length * Shift / 8;
            var result = new byte[outLength];

            var buffer = 0;
            var next = 0;
            var bitsLeft = 0;

            foreach (var c in encoded)
            {
                var charValue = CharToInt(c);
                if (charValue < 0)
                {
                    throw new FormatException($"Illegal character: {c}");
                }

                buffer <<= Shift;
                buffer |= charValue & Mask;
                bitsLeft += Shift;

                if (bitsLeft >= 8)
                {
                    result[next++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }

            return result;
        }
    }
}
