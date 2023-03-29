using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDwarf.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf
{
    public class Tileset
    {
        private static Tileset instance;

        public Texture2D TileSheet { get ; private set; }

        public static Tileset Instance
        {
            get
            {
                if (instance == null)
                    instance = new Tileset();

                return instance;
            }
        }

        public void Initialize()
        {
            AssetManager.Instance.LoadTexture("tileset");
            AssetManager.Instance.LoadTexture("isometric_tileset");

            TileSheet = AssetManager.Instance.GetTexture("isometric_tileset");
        }

        public Rectangle GetTileRectangle(int index)
        {
            int x = index % Constants.TilesetTileWidth;
            int y = index / Constants.TilesetTileWidth;
            

            return new Rectangle(x * Constants.TilesetTileWidth, y * Constants.TilesetTileHeight, Constants.TilesetTileWidth, Constants.TilesetTileHeight);
        }

        public void UnloadContent()
        {
            TileSheet.Dispose();
        }

        public void DrawTile(SpriteBatch batch, Vector3 position, int index)
        {
            DrawTile(batch, new Vector2(position.X, position.Z), index);
        }

        public void DrawTile(SpriteBatch batch, Vector2 position, int index)
        {
            batch.Draw(
                    TileSheet,
                    position,
                    GetTileRectangle(index),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Constants.ScalingFactor,
                    SpriteEffects.None,
                    1
                );
        }
    }
}
