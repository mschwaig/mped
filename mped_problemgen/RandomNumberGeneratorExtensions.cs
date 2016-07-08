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
            int result;
            do
            {
                rng.GetBytes(arr);
                arr[3] = (byte)(arr[3] & 0x7f); // eliminate sign bit
                result = BitConverter.ToInt32(arr, 0) % max_number;
            } while (result >= Int32.MaxValue - (Int32.MaxValue % max_number));

            return result;
        }
    }
}
