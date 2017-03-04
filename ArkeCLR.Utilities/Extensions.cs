using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Utilities.Extensions {
    public static class EnumExtensions {
        public static bool IsValid<T>(this T self) => Enum.IsDefined(typeof(T), self);
        public static bool IsInvalid<T>(this T self) => !Enum.IsDefined(typeof(T), self);
    }

    public static class ICollectionExtensions {
        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> range) => self.Add(range);

        public static void Add<T>(this ICollection<T> self, IEnumerable<T> range) {
            foreach (var r in range)
                self.Add(r);
        }
    }

    public static class IEnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action) {
            foreach (var s in self)
                action(s);
        }

        public static List<U> ToList<T, U>(this IEnumerable<T> self, Func<T, U> selector) => self.Select(s => selector(s)).ToList();
    }

    public static class ArrayExtensions {
        public static string ToString<T>(this T[] self, string separator, string suffix, string prefix, bool supressSuffixOnEmpty) {
            if (self.Length == 0 && supressSuffixOnEmpty) return string.Empty;
            if (self.Length == 1) return suffix + self[0].ToString() + prefix;

            return suffix + string.Join(separator, self) + prefix;
        }
    }
}
