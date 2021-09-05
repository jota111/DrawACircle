using System;
using System.Collections.Generic;

namespace SH.Util.Random
{
    public static class DynamicRandomSelectorCacheCreator
    {
        public static DynamicRandomSelectorCache<T> Create<T>()
        {
            var inst = DynamicRandomSelectorCache<T>.Create();
            return inst;
        }

        public struct DynamicRandomSelectorCache<T>
        {
            // internal buffers
            static readonly List<T> itemsList = new List<T>();
            static readonly List<float> weightsList = new List<float>();
            static readonly List<float> CDL = new List<float>(); // Cummulative Distribution List

            // internal function that gets dynamically swapped inside Build
            private static Func<List<float>, float, int> selectFunction;

            /// <summary>
            /// 생성
            /// 외부에서 new로 생성하지 말자!!!
            /// </summary>
            /// <returns></returns>
            public static DynamicRandomSelectorCache<T> Create()
            {
                Clear();
                var inst = new DynamicRandomSelectorCache<T>();
                return inst;
            }

            /// <summary>
            /// Clears internal buffers, should make no garbage (unless internal lists hold objects that aren't referenced anywhere else)
            /// </summary>
            private static void Clear()
            {
                itemsList.Clear();
                weightsList.Clear();
                CDL.Clear();
                selectFunction = null;
            }

            /// <summary>
            /// Add new item with weight into collection. Items with zero weight will be ignored.
            /// Do not add duplicate items, because removing them will be buggy (you will need to call remove for duplicates too!).
            /// Be sure to call Build() after you are done adding items.
            /// </summary>
            /// <param name="item">Item that will be returned on random selection</param>
            /// <param name="weight">Non-zero non-normalized weight</param>
            public void Add(T item, float weight)
            {
                // ignore zero weight items
                if (weight == 0)
                    return;

                itemsList.Add(item);
                weightsList.Add(weight);
            }

            /// <summary>
            /// Remove existing item with weight into collection.
            /// Be sure to call Build() after you are done removing items.
            /// </summary>
            /// <param name="item">Item that will be removed out of collection, if found</param>
            public void Remove(T item)
            {
                int index = itemsList.IndexOf(item);
                ;

                // nothing was found
                if (index == -1)
                    return;

                itemsList.RemoveAt(index);
                weightsList.RemoveAt(index);
                // no need to remove from CDL, should be rebuilt instead
            }

            /// <summary>
            /// Re/Builds internal CDL (Cummulative Distribution List)
            /// Must be called after modifying (calling Add or Remove), or it will break. 
            /// Switches between linear or binary search, depending on which one will be faster.
            /// Might generate some garbage (list resize) on first few builds.
            /// </summary>
            /// <param name="seed">You can specify seed for internal random gen or leave it alone</param>
            /// <returns>Returns itself</returns>
            private void Build()
            {
                if (itemsList.Count == 0)
                    throw new Exception("Cannot build with no items.");

                // clear list and then transfer weights
                CDL.Clear();
                for (int i = 0; i < weightsList.Count; i++)
                    CDL.Add(weightsList[i]);

                RandomMath.BuildCumulativeDistribution(CDL);

                // RandomMath.ListBreakpoint decides where to use Linear or Binary search, based on internal buffer size
                // if CDL list is smaller than breakpoint, then pick linear search random selector, else pick binary search selector
                if (CDL.Count < RandomMath.ListBreakpoint)
                    selectFunction = RandomMath.SelectIndexLinearSearch;
                else
                    selectFunction = RandomMath.SelectIndexBinarySearch;
            }

            /// <summary>
            /// Selects random item based on its probability.
            /// Uses linear search or binary search, depending on internal list size.
            /// </summary>
            /// <returns>Returns item</returns>
            public T SelectRandomItem(int seed)
            {
                Build();
                float randomValue = (float) RandomSeedUtil.NextDouble(seed);
                var index = selectFunction(CDL, randomValue);
                return itemsList[index];
            }
        }
    }
}