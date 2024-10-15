namespace LunarLander.Utilities
{
    // Miscellaneous math functions.
    static class MathKit
    {
        // Computes an integer modulo, but with no negative values.
        public static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        // Computes a float modulo, but with no negative values.
        public static float modF(float x, float m)
        {
            return (x % m + m) % m;
        }
    }
}
