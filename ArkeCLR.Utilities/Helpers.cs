namespace ArkeCLR.Utilities.Helpers {
    public static class MathEx {
        public static int RoundUpToNearestMultiple(int value, int multiple) => multiple * ((value + (multiple - 1)) / multiple);
        public static uint RoundUpToNearestMultiple(uint value, uint multiple) => multiple * ((value + (multiple - 1)) / multiple);
        public static long RoundUpToNearestMultiple(long value, long multiple) => multiple * ((value + (multiple - 1)) / multiple);
        public static ulong RoundUpToNearestMultiple(ulong value, ulong multiple) => multiple * ((value + (multiple - 1)) / multiple);
    }
}
