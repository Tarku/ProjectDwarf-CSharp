using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf
{
    public class Cuboid
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }

        public Cuboid(Vector3 position, Vector3 size)
        {
            Position = position;
            Size = size;
        }

        public Cuboid()
        {
            Position = Vector3.Zero;
            Size = new Vector3(Constants.ParcelWidth, Constants.ParcelDepth, Constants.ParcelWidth);
        }
        public Cuboid(Vector3 size)
        {
            Position = Vector3.Zero;
            Size = size;
        }
    }
}
