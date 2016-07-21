using System;
using System.Security.Cryptography;


namespace at.mschwaig.mped.problemgen
{
    static class RandomNumberGeneratorExtensions
    {
        // Fisher-Yates shuffle as described here: http://stackoverflow.com/a/1262619/2066744
        public static void Shuffle<T>(this RandomNumberGenerator rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next<RandomNumberGenerator>(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        // convert random bytes to random int in range
        public static int Next<T>(this RandomNumberGenerator rng, int max_number)
        {
            byte[] arr = new byte[4];
            int nonnegative_int;
            do
            {
                rng.GetBytes(arr);
                int any_int = BitConverter.ToInt32(arr, 0);
                nonnegative_int = Math.Abs(any_int);
            } while (nonnegative_int >= Int32.MaxValue - (Int32.MaxValue % max_number));

            return nonnegative_int % max_number;
        }

        // source: http://stackoverflow.com/a/2854635/2066744
        public static double NextDouble<T>(this RandomNumberGenerator rng)
        {
            var bytes = new Byte[8];
            rng.GetBytes(bytes);
            // Step 2: bit-shift 11 and 53 based on double's mantissa bits
            var ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
            return ul / (Double)(1UL << 53);
        }
    }
}
