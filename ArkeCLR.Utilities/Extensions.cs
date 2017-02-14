using System;
using System.Collections.Generic;

namespace ArkeCLR.Utilities.Extensions {
    public static class EnumExtensions {
        public static bool IsValid<T>(this T self) => Enum.IsDefined(typeof(T), self);
    }

    public static class ICollectionExtensions {
        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> range) => self.Add(range);

        public static void Add<T>(this ICollection<T> self, IEnumerable<T> range) {
            foreach (var r in range)
                self.Add(r);
        }
    }
}
