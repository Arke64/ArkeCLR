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
}
