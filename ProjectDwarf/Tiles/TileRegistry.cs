using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Tiles
{
    public class TileRegistry
    {
        public static List<BaseTile> Tiles = new()
        {
            new BaseTile("Air", 0, isPassable: true), // 0
            new BaseTile("Grass", 1), // 1
            new BaseTile("Sand", 2), // 2
            new BaseTile("Stone", 3), // 3
            new BaseTile("Dirt", 4), // 4
            new BaseTile("Iron Ore", 6), // 5
            new BaseTile("Pine Tree", 31), // 6
            new BaseTile("Poplar Tree", 32) // 7
        };

        public static BaseTile TileAt(Vector3 position)
        {
            return Tiles[Parcel.Instance.GetTile(position)];
        }

    }
}
