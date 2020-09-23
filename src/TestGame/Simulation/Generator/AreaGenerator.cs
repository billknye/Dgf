using System;
using System.Collections.Generic;
using System.Text;

namespace Dgf.TestGame.Simulation.Generator
{
    public class AreaGenerator
    {
        private readonly int seed;
        private readonly World world;
        private readonly int x;
        private readonly int y;

        public AreaGenerator(int seed, World world, int x, int y)
        {
            this.seed = seed;
            this.world = world;
            this.x = x;
            this.y = y;

            Random r = new Random(seed);
            AreaType areaType;
            do
            {
                areaType = (AreaType)(r.Next((int)AreaType.Max - 1) + 1);
            } while (!isValidAreaType(areaType));

            Console.WriteLine();
        }

        private bool isValidAreaType(AreaType areaType)
        {
            if (areaType == AreaType.Desert)            
                return y >= world.Height * .2
                    && y <= world.Height * .8;            

            if (areaType == AreaType.Tundra)
                return y <= world.Height * .3 || y >= world.Height * .3;

            return true;
        }
    }

    public enum AreaType
    {
        Unspecified = 0,
        Plains,
        Forest,
        Swamp,
        Mountains,
        Desert,
        Tundra,
        Max
    }
}
