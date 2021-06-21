using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace YoukaiFox.SystemExtensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Shuffles the array using the Fisher-Yates method.
        /// </summary>
        /// <param name="items"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this T[] items)
        {
            System.Random rng = new System.Random();
            
            for (int i = items.Length - 1; i > 0; i--)
            {
                int r = rng.Next(i + 1);
                Swap(ref items[i], ref items[r]);
            }
        }

        /// <summary>
        /// Shuffles the array using the Fisher-Yates method providing a seed for the random number generator.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="seed"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this T[] items, int seed)
        {
            System.Random rng = new System.Random(seed);
            
            for (int i = items.Length - 1; i > 0; i--)
            {
                int r = rng.Next(i + 1);
                Swap(ref items[i], ref items[r]);
            }
        }

        /// <summary>
        /// Get the first valid index in the collection.
        /// </summary>
        public static int GetFirstValidIndex<T>(this T[] items)
        {
            if (items.Length > 0)
                return 0;

            return -1;
        }

        /// <summary>
        /// Swap the two values provided.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <typeparam name="T"></typeparam>
        public static void Swap<T>(ref T v1, ref T v2)
        {
            T temp = v1;
            v1 = v2;
            v2 = temp;
        }
    }
}