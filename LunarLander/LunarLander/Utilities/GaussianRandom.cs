using System;

// Utility class to generate random numbers using a Gaussian distribution.
public class GaussianRandom
{
    // Random object.
    private Random random = new Random();

    // Generates a random number via Gaussian distribution.
    public double NextGaussianRandom()
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * (float)(random.NextDouble()) - 1.0f;
            v2 = 2.0f * (float)(random.NextDouble()) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = (float)Math.Sqrt((-2.0f * Math.Log(s)) / s);

        return v1 * s;
    }
}
