using Microsoft.Xna.Framework;
using System;
using LibNoise;
using LibNoise.Builder;
using LibNoise.Primitive;
using ProjectDwarf.Utils;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace ProjectDwarf
{
    public class Parcel
    {
        const int soilDepth = 6;

        List<Vector2> bezierMiddlePoints2D = new() {
            new Vector2(0, 0), new Vector2(Constants.ParcelWidth / 2, 0), new Vector2(Constants.ParcelWidth - 1, 0),
            new Vector2(0, Constants.ParcelWidth / 2), new Vector2(Constants.ParcelWidth - 1, Constants.ParcelWidth / 2),
            new Vector2(0, 1), new Vector2(Constants.ParcelWidth / 2, 1), new Vector2(Constants.ParcelWidth - 1, 1),
        };

        int seed = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private static Parcel instance;

        public byte[,,] Tiles { private set; get; }
        public byte[,] Heightmap { private set; get; }

        byte[] passableTiles = { 0 };

        NoiseMap noiseMap;
        NoiseMap detailNoiseMap;
        NoiseMap caveMap;


        public static Parcel Instance
        {
            get
            {
                if (instance == null)
                    instance = new Parcel();

                return instance;
            }
        }

        public Parcel()
        {
            Tiles = new byte[Constants.ParcelWidth, Constants.ParcelDepth, Constants.ParcelWidth];
            Heightmap = new byte[Constants.ParcelWidth, Constants.ParcelWidth];

            noiseMap = new NoiseMap(Constants.ParcelWidth, Constants.ParcelWidth);
            detailNoiseMap = new NoiseMap(Constants.ParcelWidth, Constants.ParcelWidth);
            caveMap = new NoiseMap(Constants.ParcelWidth, Constants.SurfaceLevel);

            NoiseMapBuilderPlane noiseMapBuilder = new NoiseMapBuilderPlane();
            NoiseMapBuilderPlane noiseMapBuilder2 = new NoiseMapBuilderPlane();

            ImprovedPerlin terrainPerlin = new ImprovedPerlin();
            ImprovedPerlin detailPerlin = new ImprovedPerlin();
            ImprovedPerlin cavePerlin = new ImprovedPerlin();

            terrainPerlin.Seed = seed;
            terrainPerlin.Quality = NoiseQuality.Best;

            detailPerlin.Seed = seed;
            detailPerlin.Quality = NoiseQuality.Fast;

            cavePerlin.Seed = seed;
            cavePerlin.Quality = NoiseQuality.Fast;

            noiseMapBuilder.SourceModule = terrainPerlin;
            noiseMapBuilder.SetBounds(0.0f, 5.0f, 0.0f, 5.0f);
            noiseMapBuilder.SetSize(Constants.ParcelWidth, Constants.ParcelWidth);

            noiseMapBuilder.NoiseMap = noiseMap;
            noiseMapBuilder.Build();

            noiseMapBuilder.SourceModule = detailPerlin;
            noiseMapBuilder.SetBounds(0.0f, 8.0f, 0.0f, 8.0f);
            noiseMapBuilder.SetSize(Constants.ParcelWidth, Constants.ParcelWidth);

            noiseMapBuilder.NoiseMap = detailNoiseMap;
            noiseMapBuilder.Build();

            noiseMapBuilder2.SourceModule = cavePerlin;
            noiseMapBuilder2.SetBounds(0f, 8f, 0f, 4f);
            noiseMapBuilder2.SetSize(Constants.ParcelWidth, Constants.SurfaceLevel);

            noiseMapBuilder2.NoiseMap = caveMap;
            noiseMapBuilder2.Build();
        }

        public void WorldGen()
        {
            GenerateHeightmap();

            CuboidFill(new Cuboid(new Vector3(Constants.ParcelWidth, Constants.SurfaceLevel, Constants.ParcelWidth)), 3);

            Fill(new Rectangle(0, 0, Constants.ParcelWidth, Constants.ParcelWidth), Constants.SurfaceLevel - 1, 1);
            Fill(new Rectangle(0, 0, Constants.ParcelWidth, Constants.ParcelWidth), Constants.SurfaceLevel, 17);

            SpawnOres();
            GenerateCaves();


            ApplyHeightmap();
            GenerateRavines();
        }

        void InitializeHeightmap()
        {
            for (int y = 0; y < Constants.ParcelWidth; y++)
                for (int x = 0; x < Constants.ParcelWidth; x++)
                    Heightmap[x, y] = Constants.SurfaceLevel;
        }

        void GenerateBaseHeightmap()
        {

            for (int z = 0; z < Constants.ParcelWidth; z++)
            {
                for (int x = 0; x < Constants.ParcelWidth; x++)
                {
                    byte y = (byte)(Math.Abs(noiseMap.GetValue(x, z) / 2) * (Constants.ParcelDepth - Constants.SurfaceLevel));
                    y = (byte)MathHelper.Clamp(y, 0, Constants.ParcelDepth - Constants.SurfaceLevel);

                    Heightmap[x, z] = (byte)(Constants.SurfaceLevel + y);
                }
            }
        }
        void GenerateDetailHeightmap()
        {
            for (int z = 0; z < Constants.ParcelWidth; z++)
            {
                for (int x = 0; x < Constants.ParcelWidth; x++)
                {
                    byte y = (byte)(Math.Abs(detailNoiseMap.GetValue(x, z) / 2) * (Constants.ParcelDepth - Constants.SurfaceLevel));

                    Heightmap[x, z] += (byte)y;
                }
            }
        }

        void GenerateHeightmap()
        {
            InitializeHeightmap();

            GenerateBaseHeightmap();
            GenerateDetailHeightmap();
        }

        public void CuboidFill(Cuboid volume, byte tile)
        {
            for (int y = (int)volume.Position.Y; y < (int)volume.Size.Y; y++)
                for (int z = (int)volume.Position.Z; z < (int)volume.Size.Z; z++)
                    for (int x = (int)volume.Position.X; x < (int)volume.Size.X; x++)
                        SetTile(new Vector3(x, y, z), tile);
        }
        public void Fill(Rectangle volume, int y, byte tile)
        {
            int minX;
            int minZ;

            int maxX;
            int maxZ;

            if (volume.X < volume.Size.X)
            {
                minX = volume.X;
                maxX = volume.Size.X;
            }
            else
            {
                minX = volume.Size.X;
                maxX = volume.X;
            }

            if (volume.Y < volume.Size.Y)
            {
                minZ = volume.Y;
                maxZ = volume.Size.Y;
            }
            else
            {
                minZ = volume.Size.Y;
                maxZ = volume.Y;
            }

            for (int z = minZ; z < maxZ; z++)
                for (int x = minX; x < maxX; x++)
                    if (IsInParcelBounds(new Vector3(x, y, z)))
                        SetTile(new Vector3(x, y, z), tile);
        }

        void SpawnOreBlob()
        {

            int radius = RNG.InclInt(1, 8);

            int randX = RNG.Int(radius, Constants.ParcelWidth - radius);
            int randY = RNG.Int(radius, Constants.SurfaceLevel - radius);
            int randZ = RNG.Int(radius, Constants.ParcelWidth - radius);

            SpawnBlob(new Vector3(randX, randY, randZ), 5, radius);
        }
        void SpawnBlob(Vector3 position, byte tile, int radius)
        {
            Vector3 blobCenter = position;

            for (int y = (int) position.Y - radius; y < (int) position.Y + radius; y++)
            {
                for (int z = (int) position.Z - radius; z < (int) position.Z + radius; z++)
                {
                    for (int x = (int) position.X - radius; x < (int)position.X + radius; x++)
                    {
                        Vector3 pos = new(x, y, z);

                        if (Distance.Get3D(blobCenter, pos) <= radius)
                        {
                            if (IsInParcelBounds(pos))
                            {
                                SetTile(pos, tile);
                            }
                        }
                    }
                }
            }
        }

        void SpawnOres()
        {
            int oreAmount = RNG.InclInt(500, 800);

            for (int i = 0; i < oreAmount; i++)
                SpawnOreBlob();
        }

        Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }
        void GenerateRavine()
        {
            Vector2 randomBezier2D = RNG.Choice(bezierMiddlePoints2D);
            Vector3 randomBezier3D = new Vector3(randomBezier2D.X, Constants.SurfaceLevel + soilDepth, randomBezier2D.Y);

            int randX = RNG.InclInt(5, Constants.ParcelWidth - 5);
            int randZ = RNG.InclInt(5, Constants.ParcelWidth - 5);

            int randX2 = RNG.InclInt(5, Constants.ParcelWidth - 5);
            int randZ2 = RNG.InclInt(5, Constants.ParcelWidth - 5);

            Vector3 startPosition = new Vector3(randX, Constants.SurfaceLevel + soilDepth, randZ);
            Vector3 targetPosition = new Vector3(randX2, Constants.SurfaceLevel + soilDepth, randZ2);

            int depth = RNG.InclInt(8, 25);

            int subPointsAmount = (int)MathF.Ceiling(Distance.Get3D(startPosition, targetPosition));
            float increment = 1f / subPointsAmount;

            List<Vector3> subPoints = new();

            for (float t = 0; t <= 1; t += increment)
                subPoints.Add(GetPoint(startPosition, randomBezier3D, targetPosition, t));

            foreach (Vector3 subPoint in subPoints)
            {

                for (int i = 0; i < depth; i++)
                {
                    SpawnBlob(new Vector3(subPoint.X, Constants.SurfaceLevel + 5 - i, subPoint.Z), 0, RNG.InclInt(2,4));
                }
            }
        }

        void GenerateRavines()
        {
            for (int i = 0; i < RNG.InclInt(2, 4); i++)
                GenerateRavine();
        }

        void GenerateSpaghettiCaves()
        {
            List<Vector3> cavePoints = new();

            for (int i = 0; i < RNG.InclInt(100, 300); i++)
            {
                int randX = RNG.InclInt(5, Constants.ParcelWidth - 5);
                int randY = RNG.InclInt(5, Constants.ParcelWidth - 5);
                int randZ = RNG.InclInt(5, Constants.ParcelWidth - 5);

                cavePoints.Add(
                    new Vector3(randX, randY, randZ)
                );
            }

            for (int i = 0; i < cavePoints.Count; i++)
            {
                int nextIndex;

                if (i != cavePoints.Count - 1)
                    nextIndex = i + 1;
                else
                    nextIndex = 0;

                Vector3 nextPoint = cavePoints[nextIndex];

                int subPointsAmount = (int)MathF.Ceiling(Distance.Get3D(cavePoints[i], nextPoint));
                float increment = 1f / subPointsAmount;

                List<Vector3> subPoints = new();

                for (float t = 0; t <= 1; t += increment)
                    subPoints.Add(GetPoint(cavePoints[i], nextPoint, nextPoint, t));

                foreach (Vector3 subPoint in subPoints)
                    SpawnBlob(subPoint, 0, RNG.InclInt(2, 5));
            
            }

        }
        void GenerateCaves()
        {
            GenerateSpaghettiCaves();
        }

        void ApplyHeightmap()
        {
            for (int z = 0; z < Constants.ParcelWidth; z++)
            {
                for (int x = 0; x < Constants.ParcelWidth; x++)
                {
                    byte y = Heightmap[x, z];
                    SetTile(new Vector3(x, y, z), 1);

                    int distToSurface = (int) (y - Constants.SurfaceLevel) + 1;

                    for (int i = 1; i < soilDepth; i++)
                    {
                        SetTile(new Vector3(x, y - i, z), 4);
                    }

                    for (int i = soilDepth; i <= distToSurface; i++)
                    {
                        SetTile(new Vector3(x, y - i, z), 3);
                    }

                }
            }
        }

        public void SetTile(Vector3 position, byte tile)
        {
            if (!IsInParcelBounds(position))
                return;

            Tiles[(int)position.X, (int)position.Y, (int)position.Z] = tile;
        }
        public byte GetTile(Vector3 position)
        {
            if (!IsInParcelBounds(position))
                return 0;

            byte tile = Tiles[(int)position.X, (int)position.Y, (int)position.Z];

            return tile;
        }
        public byte GetTileAndOffset(Vector3 position, byte offset)
        {
            if (GetTile(position) == 0)
                offset = 0;

            return (byte) (GetTile(position) + offset);
        }

        public Rectangle GetTileRectangleAt(Vector3 position, int offset)
        {
            if (IsInParcelBounds(position))
            {
                byte tile = Tiles[(int)position.X, (int)position.Y, (int)position.Z];

                if (tile == 0)
                    offset = 0;

                return Tileset.Instance.GetTileRectangle(tile + offset);
            } else
            {
                return Rectangle.Empty;
            }
        }

        public bool IsInParcelBounds(Vector3 position)
        {
            return (position.X >= 0 && position.Y >= 0 && position.Z >= 0 && position.X < Constants.ParcelWidth - 1 && position.Y < Constants.ParcelDepth - 1 && position.Z < Constants.ParcelWidth - 1); 
        }

        public bool IsInParcelBounds(Vector2 position, int height)
        {
            return IsInParcelBounds(new Vector3(position.X, height, position.Y));
        }

        public bool IsPassableAt(Vector3 position)
        {
            return IsInParcelBounds(position) ? Tiles[(int)position.X, (int)position.Y, (int)position.Z] == 0 : false;
        }

    }
}
