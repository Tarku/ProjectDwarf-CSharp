using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Utils
{
    public class Distance
    {
        public static float Get2D(Vector2 one, Vector2 two)
        {
            float dX = two.X - one.X;
            float dY = two.Y - one.Y;

            return MathF.Sqrt(dX * dX + dY * dY);
        }
        public static float Get3D(Vector3 one, Vector3 two)
        {
            float dX = two.X - one.X;
            float dY = two.Y - one.Y;
            float dZ = two.Z - one.Z;

            return MathF.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }
        public static float Get3D_NotPythagorean(Vector3 one, Vector3 two)
        {
            return (two.X - one.X) * (two.X - one.X) + (two.Y - one.Y) * (two.Y - one.Y) + (two.Z - one.Z) * (two.Z - one.Z);
        }
    }
}
