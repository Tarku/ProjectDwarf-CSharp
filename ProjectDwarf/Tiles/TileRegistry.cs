using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Tiles
{
    public class TileRegistry
    {
        public static List<BaseTile> Tiles = new()
        {
            new BaseTile("Air"),
            new BaseTile("Grass"),
            new BaseTile("Sand"),
            new BaseTile("Stone"),
            new BaseTile("Dirt"),
            new BaseTile("IronOre"),
            new BaseTile("IronOre"),
            new BaseTile("IronOre"),
            new BaseTile("IronOre"),
        };


    }
}
