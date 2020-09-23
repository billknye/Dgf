using System;
using System.Collections.Generic;
using System.Text;

namespace Dgf.TestGame.Simulation
{
    public class World
    {
        private readonly List<Area> areas;

        public int Width { get; set; }

        public int Height { get; set; }

        public Area this[int areaId]
        {
            get
            {
                var area = areas[areaId];
                if (area is AreaStub stub)
                {
                    area = new Area(stub.Seed, this, areaId % 4, areaId / 4);
                    areas[areaId] = area;
                }

                return area;
            }
        }

        public World(int seed)
        {
            Random r = new Random(seed);

            areas = new List<Area>(10);

            Width = 4;
            Height = 4;

            for (int i = 0; i < 16; i++)
            {
                areas.Add(new AreaStub(r.Next()));
            }
        }
    }
}
