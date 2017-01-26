using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTCommandPalette {
    public static class IEnumerableExtensions {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item) {
            foreach (T elem in enumerable) {
                yield return elem;
            }

            yield return item;
        }
    }
}
