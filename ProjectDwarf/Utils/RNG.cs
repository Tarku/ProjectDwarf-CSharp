using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf
{
    public class RNG
    {
        static Random random;

        public static void Initialize(int? seed)
        {
            if (seed == null)
                random = new Random();
            else
                random = new Random((int) seed);
        }

        public static int Int(int minValue, int maxValue)
        {
            if (random == null)
                Initialize(null);

            return random.Next(minValue, maxValue);
        }

        public static int InclInt(int minValue, int maxValue)
        {
            if (random == null)
                Initialize(null);

            return random.Next(minValue, maxValue + 1);
        }
        
        public static Vector2 Choice(List<Vector2> list)
        {
            return list[Int(0, list.Count)];
        }

        public static Vector3 Choice(List<Vector3> list)
        {
            return list[Int(0, list.Count)];
        }
    }
}
