using Dgf.TestGame.Simulation.Generator;
using System;

namespace Dgf.TestGame.Simulation
{
    public class Area
    {
        private readonly TestGameState testGameState;

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Seed { get; set; }



        /// <summary>
        /// protected ctor for sub to use
        /// </summary>
        protected Area()
        {

        }

        public Area(int seed, TestGameState testGameState, int x, int y)
        {
            this.Seed = seed;
            this.testGameState = testGameState;
            this.X = x;
            this.Y = y;

            Random r = new Random(seed);
            AreaType areaType;
            do
            {
                areaType = (AreaType)(r.Next((int)AreaType.Max - 1) + 1);
            } while (!isValidAreaType(areaType));

        }

        private bool isValidAreaType(AreaType areaType)
        {
            var world = testGameState.World;

            if (areaType == AreaType.Desert)
                return Y >= world.Height * .2
                    && Y <= world.Height * .8;

            if (areaType == AreaType.Tundra)
                return Y <= world.Height * .3 || Y >= world.Height * .3;

            return true;
        }

        public Location GetLocation(int x, int y)
        {
            return new Location
            {
                X = x,
                Y = y,
            };
        }
    }
}
