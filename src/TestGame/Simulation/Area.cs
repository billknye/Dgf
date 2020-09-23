using Dgf.TestGame.Simulation.Generator;

namespace Dgf.TestGame.Simulation
{
    public class Area
    {
        public int Seed { get; set; }

        /// <summary>
        /// protected ctor for sub to use
        /// </summary>
        protected Area()
        {

        }

        public Area(int seed, World world, int x, int y)
        {
            this.Seed = seed;

            var generator = new AreaGenerator(seed, world, x, y);
        }
    }
}
