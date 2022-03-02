﻿using System;

namespace Dgf.TestGame.Simulation.Generator
{
    public class SimplexNoise2D
    {
        public double Scale;

        double xMod, yMod;

        private static int FASTFLOOR(double x)
        {
            return ((x) > 0) ? ((int)x) : (((int)x) - 1);
        }

        //---------------------------------------------------------------------
        // Static data

        /*
         * Permutation table. This is just a random jumble of all numbers 0-255,
         * repeated twice to avoid wrapping the index at 255 for each lookup.
         * This needs to be exactly the same for all instances on all platforms,
         * so it's easiest to just keep it as static explicit data.
         * This also removes the need for any initialisation of this class.
         *
         * Note that making this an int[] instead of a char[] might make the
         * code run faster on platforms with a high penalty for unaligned single
         * byte addressing. Intel x86 is generally single-byte-friendly, but
         * some other CPUs are faster with 4-aligned reads.
         * However, a char[] is smaller, which avoids cache trashing, and that
         * is probably the most important aspect on most architectures.
         * This array is accessed a *lot* by the noise functions.
         * A vector-valued noise over 3D accesses it 96 times, and a
         * float-valued 4D noise 64 times. We want this to fit in the cache!
         */
        static byte[] perm = {151,160,137,91,90,15,
  131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
  190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
  88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
  77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
  102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
  135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
  5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
  223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
  129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
  251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
  49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
  138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
  151,160,137,91,90,15,
  131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
  190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
  88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
  77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
  102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
  135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
  5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
  223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
  129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
  251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
  49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
  138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
};

        //---------------------------------------------------------------------

        /*
         * Helper functions to compute gradients-dot-residualvectors (1D to 4D)
         * Note that these generate gradients of more than unit length. To make
         * a close match with the value range of classic Perlin noise, the final
         * noise values need to be rescaled to fit nicely within [-1,1].
         * (The simplex noise functions as such also have different scaling.)
         * Note also that these noise functions are the most practical and useful
         * signed version of Perlin noise. To return values according to the
         * RenderMan specification from the SL noise() and pnoise() functions,
         * the noise values need to be scaled and offset to [0,1], like this:
         * float SLnoise = (SimplexNoise1234::noise(x,y,z) + 1.0) * 0.5;
         */

        public static double grad(int hash, double x)
        {
            int h = hash & 15;
            double grad = 1.0f + (h & 7);   // Gradient value 1.0, 2.0, ..., 8.0
            if ((h & 8) != 0) grad = -grad;         // Set a random sign for the gradient
            return (grad * x);           // Multiply the gradient with the distance
        }

        public static double grad(int hash, double x, double y)
        {
            int h = hash & 7;      // Convert low 3 bits of hash code
            double u = h < 4 ? x : y;  // into 8 simple gradient directions,
            double v = h < 4 ? y : x;  // and compute the dot product with (x,y).
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0 * v : 2.0 * v);
        }

        public static double grad(int hash, double x, double y, double z)
        {
            int h = hash & 15;     // Convert low 4 bits of hash code into 12 simple
            double u = h < 8 ? x : y; // gradient directions, and compute dot product.
            double v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
        }

        public static double grad(int hash, double x, double y, double z, double t)
        {
            int h = hash & 31;      // Convert low 5 bits of hash code into 32 simple
            double u = h < 24 ? x : y; // gradient directions, and compute dot product.
            double v = h < 16 ? y : z;
            double w = h < 8 ? z : t;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v) + ((h & 4) != 0 ? -w : w);
        }

        // A lookup table to traverse the simplex around a given point in 4D.
        // Details can be found where this table is used, in the 4D noise method.
        /* TODO: This should not be required, backport it from Bill's GLSL code! */
        static byte[,] simplex = {
    {0,1,2,3},{0,1,3,2},{0,0,0,0},{0,2,3,1},{0,0,0,0},{0,0,0,0},{0,0,0,0},{1,2,3,0},
    {0,2,1,3},{0,0,0,0},{0,3,1,2},{0,3,2,1},{0,0,0,0},{0,0,0,0},{0,0,0,0},{1,3,2,0},
    {0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},
    {1,2,0,3},{0,0,0,0},{1,3,0,2},{0,0,0,0},{0,0,0,0},{0,0,0,0},{2,3,0,1},{2,3,1,0},
    {1,0,2,3},{1,0,3,2},{0,0,0,0},{0,0,0,0},{0,0,0,0},{2,0,3,1},{0,0,0,0},{2,1,3,0},
    {0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},
    {2,0,1,3},{0,0,0,0},{0,0,0,0},{0,0,0,0},{3,0,1,2},{3,0,2,1},{0,0,0,0},{3,1,2,0},
    {2,1,0,3},{0,0,0,0},{0,0,0,0},{0,0,0,0},{3,1,0,2},{0,0,0,0},{3,2,0,1},{3,2,1,0}};

        // 1D simplex noise
        public static double noise(double x)
        {

            int i0 = FASTFLOOR(x);
            int i1 = i0 + 1;
            double x0 = x - i0;
            double x1 = x0 - 1.0f;

            double n0, n1;

            double t0 = 1.0f - x0 * x0;
            //  if(t0 < 0.0f) t0 = 0.0f;
            t0 *= t0;
            n0 = t0 * t0 * grad(perm[i0 & 0xff], x0);

            double t1 = 1.0f - x1 * x1;
            //  if(t1 < 0.0f) t1 = 0.0f;
            t1 *= t1;
            n1 = t1 * t1 * grad(perm[i1 & 0xff], x1);
            // The maximum value of this noise is 8*(3/4)^4 = 2.53125
            // A factor of 0.395 would scale to fit exactly within [-1,1], but
            // we want to match PRMan's 1D noise, so we scale it down some more.
            return 0.25 * (n0 + n1);

        }

        public const double F2 = 0.366025403f; // F2 = 0.5*(sqrt(3.0)-1.0)
        public const double G2 = 0.211324865f; // G2 = (3.0-Math.sqrt(3.0))/6.0

        #region INoise2D Members

        public SimplexNoise2D(int? seed, double scale = 100)
        {
            this.Scale = scale;

            Random r;

            if (seed.HasValue)
            {
                r = new Random(seed.Value);
            }
            else
            {
                r = new Random();
            }

            xMod = r.NextDouble() * 82482.0;
            yMod = r.NextDouble() * 475643.0;

        }

        public double GetValue(int xi, int yi)
        {
            double x = xi / Scale + xMod;
            double y = yi / Scale + yMod;

            double n0, n1, n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in
            double s = (x + y) * F2; // Hairy factor for 2D
            double xs = x + s;
            double ys = y + s;
            int i = FASTFLOOR(xs);
            int j = FASTFLOOR(ys);

            double t = (double)(i + j) * G2;
            double X0 = i - t; // Unskew the cell origin back to (x,y) space
            double Y0 = j - t;
            double x0 = x - X0; // The x,y distances from the cell origin
            double y0 = y - Y0;

            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else { i1 = 0; j1 = 1; }      // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6

            double x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            double y1 = y0 - j1 + G2;
            double x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
            double y2 = y0 - 1.0f + 2.0f * G2;

            // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
            int ii = i % 256;
            int jj = j % 256;

            // Calculate the contribution from the three corners
            double t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * grad(perm[ii + perm[jj]], x0, y0);
            }

            double t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * grad(perm[ii + i1 + perm[jj + j1]], x1, y1);
            }

            double t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * grad(perm[ii + 1 + perm[jj + 1]], x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            double ret = 0.5f + 23.4129f * (n0 + n1 + n2); // TODO: The scale factor is preliminary!

            return Math.Max(0, Math.Min(ret, 1.0f));
        }

        #endregion
    }
}
