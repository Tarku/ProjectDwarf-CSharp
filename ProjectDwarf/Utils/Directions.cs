using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Utils
{
    public class Directions2D
    {
        public static readonly Vector3 UpLeft1 = new Vector3(-1, 1, -1);
        public static readonly Vector3 Up1 = new Vector3(0, 1, -1);
        public static readonly Vector3 UpRight1 = new Vector3(1, 1, -1);

        public static readonly Vector3 DownLeft1 = new Vector3(-1, 1, 1);
        public static readonly Vector3 Down1 = new Vector3(0, 1, 1);
        public static readonly Vector3 DownRight1 = new Vector3(1, 1, 1);

        public static readonly Vector3 Left1 = new Vector3(-1, 1, 0);
        public static readonly Vector3 Right1 = new Vector3(1, 1, 0);

        public static readonly Vector3 UpLeft = new Vector3(-1, 0, -1);
        public static readonly Vector3 Up = new Vector3(0, 0, -1);
        public static readonly Vector3 UpRight = new Vector3(1, 0, -1);

        public static readonly Vector3 DownLeft = new Vector3(-1, 0, 1);
        public static readonly Vector3 Down = new Vector3(0, 0, 1);
        public static readonly Vector3 DownRight = new Vector3(1, 0, 1);

        public static readonly Vector3 Left = new Vector3(-1, 0, 0);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);

        public static readonly Vector3 UpLeft_1 = new Vector3(-1, -1, - 1);
        public static readonly Vector3 Up_1 = new Vector3(0, -1, -1);
        public static readonly Vector3 UpRight_1 = new Vector3(1, -1, -1);

        public static readonly Vector3 DownLeft_1 = new Vector3(-1, -1, 1);
        public static readonly Vector3 Down_1 = new Vector3(0, -1, 1);
        public static readonly Vector3 DownRight_1 = new Vector3(1, - 1, 1);

        public static readonly Vector3 Left_1 = new Vector3(-1, -1, 0);
        public static readonly Vector3 Right_1 = new Vector3(1, -1, 0);

        public static readonly List<Vector3> All = new() {
            UpLeft1, Up1, UpRight1,
            Left1, Right1,
            DownLeft1, Down1, DownRight1,

            UpLeft, Up, UpRight,
            Left, Right,
            DownLeft, Down, DownRight,

            UpLeft_1, Up_1, UpRight_1,
            Left_1, Right_1,
            DownLeft_1, Down_1, DownRight_1
        };
    }
}
