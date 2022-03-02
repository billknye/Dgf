using System;
using System.Collections.Generic;
using System.Text;

namespace Dgf.TestGame.Simulation.Generator;

public class WrappingPerlinNoise2D 
{
    int r1, r2, r3;

    double frequency, persistence, octaves, amplitude;

    public double WrapWidth = 16777216;

    public double Minimum, Maximum;

    public WrappingPerlinNoise2D(int seed, double freq = 0.01, double pers = 0.5, double octaves = 5, double amplitude = 8)
    {
        Random r;
        r = new Random(seed);
        r1 = r.Next(1000, 10000);
        r2 = r.Next(100000, 1000000);
        r3 = r.Next(1000000000, 2000000000);

        frequency = freq;
        persistence = pers;
        this.octaves = octaves;
        this.amplitude = amplitude;
    }

    double Smooth(double x, double y)
    {
        double n1 = Noise((int)x, (int)y);
        double n2 = Noise((int)(x + 1), (int)y);
        double n3 = Noise((int)x, (int)y + 1);
        double n4 = Noise((int)(x + 1), (int)y + 1);

        double i1 = Interpolate(n1, n2, x - (int)x);
        double i2 = Interpolate(n3, n4, x - (int)x);

        return Interpolate(i1, i2, y - (int)y);
    }



    double Noise(int x, int y)
    {
        x = x % (int)(WrapWidth * frequency);

        int n = (x + y * 57);
        n = (n << 13) ^ n;

        return (1.0 - ((n * (n * n * r1 + r2) + r3) & 0x7fffffff) / 1073741824.0);
    }

    double Interpolate(double x, double y, double a)
    {
        double val = (1 - Math.Cos(a * Math.PI)) * .5;
        return x * (1 - val) + y * val;
    }

    public double GetValue(int x, int y)
    {
        //x += 1073741823;
        //y += 1073741823;

        double total = 0.0;

        double frequency = this.frequency; // USER ADJUSTABLE
        double persistence = this.persistence; // USER ADJUSTABLE
        double octaves = this.octaves; // USER ADJUSTABLE
        double amplitude = this.amplitude; // USER ADJUSTABLE

        for (int lcv = 0; lcv < octaves; lcv++)
        {
            //total = total + Smooth(x * frequency, y * frequency) * amplitude;
            total = total + Smooth(x * frequency, y * frequency) * amplitude;
            frequency = frequency * 2;
            amplitude = amplitude * persistence;
        }

        double cloudCoverage = 0; // USER ADJUSTABLE
        double cloudDensity = .1; // USER ADJUSTABLE

        total = (total + cloudCoverage) * cloudDensity;

        if (Minimum > total)
            Minimum = total;

        if (Maximum < total)
            Maximum = total;

        //total = Math.Max(-1, total);
        //total = Math.Min(1, total);

        //if (total < 0) total = 0.0;
        //if (total > 1) total = 1.0;

        return total;
    }

}
