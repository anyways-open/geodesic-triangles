using System;

namespace Anyways.GeodesicTriangles.Internal
{
    /// <summary>
    /// Converts an QTM-id into a int32 or int64number
    /// </summary>
    public static class IdEncoder
    {
        public static int[] Decode(this uint l)
        {
            return ((ulong) l).Decode();
        }

        public static int[] Decode(this ulong l)
        {
            // Search the start
            var maxDigits = 30;
            // 7 = 111
            // Shifted by maxDigits*2: 00 0111 00 00 00 ...
            // The id might be         00 0OCT QA QA QA
            //                         &&&&&&&&&&&&&&&&
            //                         00 0OCT 00 00 00
            // IF -------------------------^^^ is not zero, we have found our precision and first index
            while ((l & (15ul << ((maxDigits) * 2))) == 0)
            {
                maxDigits--;
            }

            var id = new int[maxDigits];
            id[0] = (int) (l >> (maxDigits - 1) * 2) - 7; // Remove the first bit (thus minus eight) and add one bit
            for (int i = 1; i < maxDigits; i++)
            {
                var shifted = l >> ((maxDigits - 1 - i) * 2);
                var truncated = shifted & 3;
                id[i] = (int) truncated;
            }

            return id;
        }
        public static uint EncodeInt(this int[] id)
        {
            // We need:
            // One bit indicating the start
            // 3 bits to encode the octant
            // 2 bits per digit
            // An has 32 bits, resulting in 14 identifiers that can be stored
            if (id.Length > 14)
            {
                throw new ArgumentException("ID contains more then 14 digits, but saving to int only supports at most 14");
            }

            var l = (uint) (id[0] - 1) | 8;
            for (int i = 1; i < id.Length; i++)
            {
                l = (l << 2) | (uint) id[i];
            }
            return l;
        }
        public static ulong EncodeLong(this int[] id)
        {
            // We need:
            // One bit indicating the start
            // 2 bits to encode the octant
            // 2 bits per digit
            // A long has 64 bits, resulting in 30 identifiers that can be stored
            if (id.Length > 31)
            {
                throw new ArgumentException("ID contains more then 30 digits, but saving to long only supports at most 30");
            }

            var l = (ulong) (id[0] - 1) | 8;

            for (int i = 1; i < id.Length; i++)
            {
                l = (l << 2) | (ulong) id[i];
            }

            return l;
        }
    }
}