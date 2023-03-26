using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf
{
    public static class Constants
    {
        public static int ScreenWidth = 1440;
        public static int ScreenHeight = 720;

        public static Vector2 ScreenBounds = new Vector2(ScreenHeight, ScreenWidth);

        public static Vector2 HalfScreen = new Vector2(ScreenWidth / 2, ScreenHeight / 2);

        public static readonly bool Fullscreen = false;

        public static readonly byte ParcelWidth = 255;
        public static readonly byte ParcelDepth = 255;

        public static readonly Vector2 UIPadding = new(10);

        public static readonly byte SurfaceLevel = (byte) (0.5 * ParcelDepth);

        public static readonly float CameraSpeed = 20f;
        public static float ScalingFactor = 1.75f;

        public static readonly int TilesetTileSize = 16;
        public static readonly int PopulationCap = 200;

        public static readonly int TileSize = (int) (TilesetTileSize * ScalingFactor);
    }
}
