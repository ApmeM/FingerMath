namespace FingerMath.Matching
{
    using System;
    using System.Collections.Generic;

    public static class TypeBitSetManager
    {
        private static readonly Dictionary<Type, int> ComponentTypesMask = new Dictionary<Type, int>();

        public static int GetIndexFor(Type type)
        {
            if (!ComponentTypesMask.ContainsKey(type))
            {
                ComponentTypesMask[type] = ComponentTypesMask.Count;
            }

            return ComponentTypesMask[type];
        }

        public static IEnumerable<Type> GetTypesFromBits(BitSet bits)
        {
            foreach (var keyValuePair in ComponentTypesMask)
            {
                if (bits.Get(keyValuePair.Value))
                {
                    yield return keyValuePair.Key;
                }
            }
        }

        public static BitSet GetBitsFromTypes(IEnumerable<Type> types)
        {
            BitSet result = new BitSet();
            foreach (var type in types)
            {
                result.Set(GetIndexFor(type));
            }

            return result;
        }
    }
}

