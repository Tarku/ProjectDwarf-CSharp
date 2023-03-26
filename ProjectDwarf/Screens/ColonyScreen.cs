using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDwarf.Managers;
using System;
using System.Collections.Generic;
using ProjectDwarf.UI;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Screens
{
    public class ColonyScreen : BaseScreen
    {
        SpriteFont font;
        int currentLevel = Constants.SurfaceLevel;

        Rectangle mouseSelection = new Rectangle();

        bool isSelectionActive = false;
        bool isLevelScrollingButtonActive = false;
        bool followDwarf = false;

        int worldZ;
        int worldX;

        int groundTileOffset = 0;
        int groundTileLevelOffset = -1;

        int maxScreenZ;
        int maxScreenX;

        GameTime gameTime;
        SpriteBatch spriteBatch;

        int frameRate;

        DesignationTypes currentDesignationType = DesignationTypes.None;

        Vector3 flagPosition;
        Vector2 cameraPosition;

        int currentDwarfID = 0;

        public ColonyScreen(SpriteBatch spriteBatch) { this.spriteBatch = spriteBatch; }

        public override void LoadContent()
        {
            AssetManager.Instance.LoadFont();

            font = AssetManager.Instance.GetFont();

            AssetManager.Instance.LoadTexture("text_background");

            Colony.Instance.Name = "New Colony";

            currentLevel = (int) Colony.Instance.Members[currentDwarfID].Position.Y;
        }

        public override void UnloadContent()
        {
            Tileset.Instance.UnloadContent();
        }

        void HandleSelection(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !isSelectionActive)
            {
                Vector2 mousePosition = Mouse.GetState().Position.ToVector2() / Constants.TileSize;

                mousePosition += cameraPosition;

                mouseSelection.Location = new Vector2(mousePosition.X, mousePosition.Y).ToPoint();

                isSelectionActive = true;
            }

            if (isSelectionActive)
            {
                Vector2 mousePosition = Mouse.GetState().Position.ToVector2() / Constants.TileSize;

                mousePosition += cameraPosition;

                mouseSelection.Size = new Point((int)mousePosition.X, (int)mousePosition.Y);

                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    Parcel.Instance.Fill(mouseSelection, currentLevel, 0);
                    isSelectionActive = false;
                }
            }
        }

        void KeepCameraInBounds()
        {
            if (cameraPosition.X < 0)
                cameraPosition.X = 0;

            if (cameraPosition.Y < 0)
                cameraPosition.Y = 0;

            if (cameraPosition.X >= Constants.ParcelWidth - Constants.ScreenWidth / Constants.TileSize - 1)
                cameraPosition.X = Constants.ParcelWidth - Constants.ScreenWidth / Constants.TileSize - 2;

            if (cameraPosition.Y >= Constants.ParcelWidth - Constants.ScreenHeight / Constants.TileSize - 1)
                cameraPosition.Y = Constants.ParcelWidth - Constants.ScreenHeight / Constants.TileSize - 2;
        }

        void HandleLevelScrolling(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.PageDown) && !isLevelScrollingButtonActive)
                isLevelScrollingButtonActive = true;

            if (keyboardState.IsKeyUp(Keys.PageDown) && isLevelScrollingButtonActive)
            {
                currentLevel--;
                followDwarf = false;
                isLevelScrollingButtonActive = false;
            }

            if (keyboardState.IsKeyDown(Keys.PageUp) && !isLevelScrollingButtonActive)
                isLevelScrollingButtonActive = true;

            if (keyboardState.IsKeyUp(Keys.PageUp) && isLevelScrollingButtonActive)
            {
                currentLevel++;
                followDwarf = false;
                isLevelScrollingButtonActive = false;
            }

            currentLevel = MathHelper.Clamp(currentLevel, 0, Constants.ParcelDepth - 1);
        }

        void HandleDwarfPanning(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.E))
                currentDwarfID++;
            
            if (keyboard.IsKeyDown(Keys.A))
                currentDwarfID--;

            if (currentDwarfID < 0)
                currentDwarfID = (byte) Colony.Instance.Members.Count;

            if (currentDwarfID >= (byte)Colony.Instance.Members.Count)
                currentDwarfID = 0;

            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.E))
            {
                float dwarfPosX = Colony.Instance.Members[currentDwarfID].Position.X;
                float dwarfPosZ = Colony.Instance.Members[currentDwarfID].Position.Z;

                followDwarf = false;

                cameraPosition = new Vector2(dwarfPosX, dwarfPosZ) - Constants.HalfScreen / Constants.TileSize ;

                currentLevel = (byte) Colony.Instance.Members[currentDwarfID].Position.Y;
            }

            if (keyboard.IsKeyDown(Keys.F))
            {
                followDwarf = true;
            }
        }

        void HandleCameraUpdate(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                cameraPosition += new Vector2(0, -1) * deltaTime * Constants.CameraSpeed;
                KeepCameraInBounds();
                followDwarf = false;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                cameraPosition += new Vector2(0, 1) * deltaTime * Constants.CameraSpeed;
                KeepCameraInBounds();
                followDwarf = false;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                cameraPosition += new Vector2(-1, 0) * deltaTime * Constants.CameraSpeed;
                KeepCameraInBounds();
                followDwarf = false;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                cameraPosition += new Vector2(1, 0) * deltaTime * Constants.CameraSpeed;
                KeepCameraInBounds();
                followDwarf = false;
            }

            maxScreenZ = (int)(cameraPosition.Y / Constants.TileSize + Constants.ScreenHeight / Constants.TileSize) + 1;
            maxScreenX = (int)(cameraPosition.X / Constants.TileSize + Constants.ScreenWidth / Constants.TileSize) + 1;

        }

        public override void Update(GameTime gameTime)
        {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.gameTime = gameTime;
            HandleSelection(gameTime);
            HandleLevelScrolling(gameTime);
            HandleDwarfPanning(gameTime);
            HandleCameraUpdate(gameTime);

            Colony.Instance.Update(gameTime);

            if (followDwarf)
            {
                float dwarfPosX = Colony.Instance.Members[currentDwarfID].Position.X;
                float dwarfPosZ = Colony.Instance.Members[currentDwarfID].Position.Z;

                Vector2 camPos = new Vector2(dwarfPosX, dwarfPosZ) - Constants.HalfScreen / Constants.TileSize;

                cameraPosition = camPos;

                currentLevel = (int) Colony.Instance.Members[currentDwarfID].Position.Y;
            }
        }

        void DrawSelectionTiles()
        {
            if (!isSelectionActive)
                return;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);

            int minX;
            int minZ;

            int maxX;
            int maxZ;

            if (mouseSelection.X < mouseSelection.Size.X)
            {
                minX = mouseSelection.X;
                maxX = mouseSelection.Size.X;
            }
            else
            {
                minX = mouseSelection.Size.X;
                maxX = mouseSelection.X;
            }

            if (mouseSelection.Y < mouseSelection.Size.Y)
            {
                minZ = mouseSelection.Y;
                maxZ = mouseSelection.Size.Y;
            }
            else
            {
                minZ = mouseSelection.Size.Y;
                maxZ = mouseSelection.Y;
            }

            if (mouseSelection.Y == mouseSelection.Size.Y)
            {
                minZ = mouseSelection.Y;
                maxZ = mouseSelection.Y;
            }
            if (mouseSelection.X == mouseSelection.Size.X)
            {
                minX = mouseSelection.X;
                maxX = mouseSelection.X;
            }

            spriteBatch.End();
        }

        void DrawTiles()
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int screenZ = 0; screenZ <= maxScreenZ; screenZ++)
            {
                for (int screenX = 0; screenX <= maxScreenX; screenX++)
                {
                    worldX = screenX + (int) cameraPosition.X;
                    worldZ = screenZ + (int) cameraPosition.Y;

                    if (!Parcel.Instance.IsInParcelBounds(new Vector2(worldX, worldZ), currentLevel) || !Parcel.Instance.IsInParcelBounds(new Vector2(worldX, worldZ), currentLevel - 1))
                        continue;

                    groundTileOffset = Parcel.Instance.Tiles[worldX, currentLevel, worldZ] == 0 ? 16 : 0;
                    groundTileLevelOffset = Parcel.Instance.Tiles[worldX, currentLevel, worldZ] == 0 ? -1 : 0;

                    Tileset.Instance.DrawTile(spriteBatch, new Vector2(screenX, screenZ) * Constants.TileSize, Parcel.Instance.GetTileAndOffset(new Vector3(worldX, currentLevel + groundTileLevelOffset, worldZ), (byte) groundTileOffset));

                    foreach (Dwarf dwarf in Colony.Instance.Members.Where(item => item.Position == new Vector3(worldX, currentLevel, worldZ)))
                    {
                        Tileset.Instance.DrawTile(spriteBatch, new Vector2(screenX, screenZ) * Constants.TileSize, 16);
                    }
                }
            }

            spriteBatch.End();
        }

        void DrawUI()
        {
            spriteBatch.Begin();

            UIText.Display($"Colony {Colony.Instance.Name} ({Colony.Instance.Members.Count} members)", Color.White, new Vector2(), UIAnchor.CenterUp); 
            UIText.Display($"{frameRate} fps", Color.Yellow, new Vector2(), UIAnchor.UpRight, true);

            UIText.Display($"{currentLevel - Constants.SurfaceLevel}", Color.Lime, new Vector2(), UIAnchor.DownRight, true);

            if (followDwarf)
            {
                int dwarfPosX = (int) Colony.Instance.Members[currentDwarfID].Position.X;
                int dwarfPosY = (int) Colony.Instance.Members[currentDwarfID].Position.Y;
                int dwarfPosZ = (int) Colony.Instance.Members[currentDwarfID].Position.Z;

                string followString = $"Following {Colony.Instance.Members[currentDwarfID].Name} (X: {dwarfPosX}; Y: {dwarfPosY - Constants.SurfaceLevel} ; Z: {dwarfPosZ})"; ;

                UIText.Display(followString, Color.White, new Vector2(), UIAnchor.CenterDown, true);
            }

            spriteBatch.End();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            DrawTiles();
            DrawUI();

            frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
