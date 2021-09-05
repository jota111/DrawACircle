/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace SH.Util.Random
{
    /// <summary>
    /// seed 재활용 가능한 랜덤 기본 Random에서 가져왔음
    /// </summary>
    public static class RandomSeedUtil
    {
        private const int MBIG = 2147483647;
        private const int MSEED = 161803398;
        private const int MZ = 0;
        private static int inext;
        private static int inextp;
        private static readonly int[] SeedArray = new int[56];

        private static void SetSeed(int Seed)
        {
            int num1 = 161803398 - (Seed == int.MinValue ? int.MaxValue : Math.Abs(Seed));
            SeedArray[55] = num1;
            int num2 = 1;
            for (int index1 = 1; index1 < 55; ++index1)
            {
                int index2 = 21 * index1 % 55;
                SeedArray[index2] = num2;
                num2 = num1 - num2;
                if (num2 < 0)
                    num2 += int.MaxValue;
                num1 = SeedArray[index2];
            }
            for (int index1 = 1; index1 < 5; ++index1)
            {
                for (int index2 = 1; index2 < 56; ++index2)
                {
                    SeedArray[index2] -= SeedArray[1 + (index2 + 30) % 55];
                    if (SeedArray[index2] < 0)
                        SeedArray[index2] += int.MaxValue;
                }
            }
            inext = 0;
            inextp = 21;
        }
        
        private static int InternalSample()
        {
            int temp_inext = inext;
            int temp_inextp = inextp;
            int index1;
            if ((index1 = temp_inext + 1) >= 56)
                index1 = 1;
            int index2;
            if ((index2 = temp_inextp + 1) >= 56)
                index2 = 1;
            int num = SeedArray[index1] - SeedArray[index2];
            if (num == int.MaxValue)
                --num;
            if (num < 0)
                num += int.MaxValue;
            SeedArray[index1] = num;
            inext = index1;
            inextp = index2;
            return num;
        }
        
        private static double Sample() => (double) InternalSample() * 4.6566128752458E-10;
        
        private static double GetSampleForLargeRange()
        {
            int num = InternalSample();
            if ((InternalSample() % 2 == 0 ? 1 : 0) != 0)
                num = -num;
            return ((double) num + 2147483646.0) / 4294967293.0;
        }

        /// <summary>
        /// 0.0보다 크거나 같고 1.0보다 작은 부동 소수점 난수입니다.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static double NextDouble(int seed)
        {
            SetSeed(seed);
            return Sample();
        }

        /// <summary>음수가 아닌 임의의 정수를 반환합니다.</summary>
        /// <returns>
        ///   보다 큰 또는 0 같고 작은 32 비트 부호 있는 정수 보다 <see cref="F:System.Int32.MaxValue" />합니다.
        /// </returns>
        public static int Next(int seed)
        {
            SetSeed(seed);
            return InternalSample();
        }
        
        /// <summary>지정된 범위 내의 임의의 정수를 반환합니다.</summary>
        /// <param name="minValue">임의의 수의 경계값 반환 됩니다.</param>
        /// <param name="maxValue">
        ///   반환 되는 임의의 수의 단독 상한입니다.
        ///   <paramref name="maxValue" />보다 크거나 해야 <paramref name="minValue" />합니다.
        /// </param>
        /// <returns>
        ///   32 비트 부호 있는 정수 보다 크거나 <paramref name="minValue" /> 및 보다 작은 <paramref name="maxValue" />; 즉, 반환 값의 범위에 포함 되어 <paramref name="minValue" /> 아닌 <paramref name="maxValue" />합니다.
        ///    경우 <paramref name="minValue" /> equals <paramref name="maxValue" />, <paramref name="minValue" /> 반환 됩니다.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="minValue" />가 <paramref name="maxValue" />보다 큰 경우
        /// </exception>
        public static int Next(int seed, int minValue, int maxValue)
        {
            SetSeed(seed);
            
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof (minValue), $"{nameof(minValue)}' cannot be greater than {nameof(maxValue)}");
            long num = (long) maxValue - (long) minValue;
            return num <= (long) int.MaxValue ? (int) (Sample() * (double) num) + minValue : (int) ((long) (GetSampleForLargeRange() * (double) num) + (long) minValue);
        }

        /// <summary>
        /// seed값으로 랜덤 항목 가져오기
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="seed"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static T RandomSelect<T>([NotNull]this IEnumerable<T> enumerable, int seed) where T : class
        {
            var count = enumerable.Count();
            if (count == 0)
                return null;

            var index = Next(seed, 0, count);
            var result = enumerable.ElementAtOrDefault(index);
            return result;
        }

        /// <summary>
        /// 가중치 기반의 랜덤 선택
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="seed"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomSelectByWeight<T>([NotNull] this IEnumerable<(T, float)> enumerable, int seed)
        {
            var selector = DynamicRandomSelectorCacheCreator.Create<T>();
            foreach (var (value, weight) in enumerable)
            {
                selector.Add(value, weight);
            }
            var randomItem = selector.SelectRandomItem(seed);
            return randomItem;
        }
        
        /// <summary>
        /// 가중치 기반의 랜덤 선택
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="seed"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomSelectByWeight<T>([NotNull] this IEnumerable<Tuple<T, int>> enumerable, int seed)
        {
            var selector = DynamicRandomSelectorCacheCreator.Create<T>();
            foreach (var (value, weight) in enumerable)
            {
                selector.Add(value, weight);
            }
            var randomItem = selector.SelectRandomItem(seed);
            return randomItem;
        }
    }
}