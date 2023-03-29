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
using ProjectDwarf.Tiles;
using ProjectDwarf.Utils;

namespace ProjectDwarf.Screens
{
    public class ColonyScreen : BaseScreen
    {
        int currentLevel = Constants.SurfaceLevel;

        Rectangle mouseSelection = new Rectangle();

        bool isSelectionActive = false;
        bool isLevelScrollingButtonActive = false;
        bool followDwarf = false;

        int worldZ;
        int worldX;

        int groundTileOffset = 0;
        int groundTileLevelOffset = -1;

        Vector2 drawOffset = new Vector2(0, Constants.TileHeight / 3);
        int layersDisplayedAtOnce = 4;

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
            AssetManager.Instance.LoadFonts();


            Colony.Instance.Name = "New Colony";

            currentLevel = (int) Colony.Instance.Members[currentDwarfID].Position.Y;
        }

        public override void UnloadContent()
        {
            Tileset.Instance.UnloadContent();
        }

        Vector2 ScreenToMapPosition(Vector2 position)
        {
            Vector2 worldPosition = new Vector2();

            position.X -= Constants.HalfScreen.X - (Constants.TileWidth / 2);

            worldPosition.X = 0.5f * (position.X / (Constants.TileWidth / 2) + position.Y / (Constants.TileHeight / 3));
            worldPosition.Y = 0.5f * (-position.X / (Constants.TileWidth / 2) + position.Y / (Constants.TileHeight / 3));

            worldPosition += cameraPosition;

            return worldPosition;
        }

        void HandleSelection(GameTime gameTime)
        {

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !isSelectionActive)
            {
                Vector2 mousePosition = Mouse.GetState().Position.ToVector2();
                Vector2 worldMousePosition = ScreenToMapPosition(mousePosition);

                mouseSelection.Location = new Point((int)Math.Floor(worldMousePosition.X), (int)Math.Floor(worldMousePosition.Y - 1));

                isSelectionActive = true;
            }

            if (isSelectionActive)
            {
                Vector2 mousePosition = Mouse.GetState().Position.ToVector2();
                Vector2 worldMousePosition = ScreenToMapPosition(mousePosition);

                mouseSelection.Size = new Point((int)Math.Ceiling(worldMousePosition.X), (int)Math.Ceiling(worldMousePosition.Y)) - mouseSelection.Location;

                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {

                    isSelectionActive = false;

                    if (currentDesignationType == DesignationTypes.Mine)
                    {
                        int totalTiles = 0;

                        for (int z = mouseSelection.Location.Y; z < mouseSelection.Size.Y + mouseSelection.Location.Y; z++)
                            for (int x = mouseSelection.Location.X; x < mouseSelection.Size.X + mouseSelection.Location.X; x++)
                                if (TileRegistry.TileAt(new Vector3(x, currentLevel, z)) != TileRegistry.Tiles[0])
                                {
                                    TaskManager.Instance.AddTask(new Tasks.MiningTask(new Vector3(x, currentLevel, z)));
                                    totalTiles++;
                                }

                        Console.WriteLine($"Mining task initiated ({totalTiles} selected)");

                    }

                }
            }
        }

        void KeepCameraInBounds()
        {
            if (cameraPosition.X < 0)
                cameraPosition.X = 0;

            if (cameraPosition.Y < 0)
                cameraPosition.Y = 0;

            if (cameraPosition.X >= Constants.ParcelWidth - Constants.ScreenWidth / Constants.TileWidth - 1)
                cameraPosition.X = Constants.ParcelWidth - Constants.ScreenWidth / Constants.TileWidth - 2;

            if (cameraPosition.Y >= Constants.ParcelWidth - Constants.ScreenHeight / Constants.TileWidth - 1)
                cameraPosition.Y = Constants.ParcelWidth - Constants.ScreenHeight / Constants.TileWidth - 2;
        }

        void HandleLevelScrolling(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.OemBackslash) && !keyboardState.IsKeyDown(Keys.LeftShift) && !isLevelScrollingButtonActive)
                isLevelScrollingButtonActive = true;

            if (keyboardState.IsKeyUp(Keys.OemBackslash) && !keyboardState.IsKeyDown(Keys.LeftShift) && isLevelScrollingButtonActive)
            {
                currentLevel++;
                followDwarf = false;
                isLevelScrollingButtonActive = false;
            }

            if (keyboardState.IsKeyDown(Keys.OemBackslash) && keyboardState.IsKeyDown(Keys.LeftShift) && !isLevelScrollingButtonActive)
                isLevelScrollingButtonActive = true;

            if (keyboardState.IsKeyUp(Keys.OemBackslash) && keyboardState.IsKeyDown(Keys.LeftShift) && isLevelScrollingButtonActive)
            {
                currentLevel--;
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


                cameraPosition = new Vector2(dwarfPosX, dwarfPosZ) - Constants.HalfScreen / Constants.TileWidth ;

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
                //KeepCameraInBounds();
                followDwarf = false;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                cameraPosition += new Vector2(0, 1) * deltaTime * Constants.CameraSpeed;
                //KeepCameraInBounds();
                followDwarf = false;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                cameraPosition += new Vector2(-1, 0) * deltaTime * Constants.CameraSpeed;
                //KeepCameraInBounds();
                followDwarf = false;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                cameraPosition += new Vector2(1, 0) * deltaTime * Constants.CameraSpeed;
                //KeepCameraInBounds();
                followDwarf = false;
            }

            maxScreenZ = (int)(cameraPosition.Y + Constants.ScreenHeight / Constants.TileWidth);
            maxScreenX = (int)(cameraPosition.X + Constants.ScreenWidth / Constants.TileWidth);

        }

        public override void Update(GameTime gameTime)
        {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.gameTime = gameTime;

            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.M))
            {
                currentDesignationType = DesignationTypes.Mine;
                Console.WriteLine("Mining task selected.");
            }

            HandleSelection(gameTime);
            HandleLevelScrolling(gameTime);
            HandleDwarfPanning(gameTime);
            HandleCameraUpdate(gameTime);

            Colony.Instance.Update(gameTime);

            if (followDwarf)
            {
                float dwarfPosX = Colony.Instance.Members[currentDwarfID].Position.X;
                float dwarfPosZ = Colony.Instance.Members[currentDwarfID].Position.Z;

                Vector2 camPos = new Vector2(dwarfPosX, dwarfPosZ) - Constants.HalfScreen / Constants.TileWidth / 2;

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
            Vector2 screenPosition = new Vector2();
            Vector2 mapPosition = new Vector2();

            for (int i = layersDisplayedAtOnce - 3; i >= 0; i--)
            {
                int minY = Constants.ScreenHeight / Constants.TileHeight;
                int minX = Constants.ScreenWidth / Constants.TileWidth / 2;

                int maxY = (int)(minY * 2.2f * 1.5f);
                int maxX = (int)(minX * 2.2f);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

                for (int screenY = (int)(-minY * 1.2f * 1.5f - i); screenY < maxY + i; screenY++)
                {
                    for (int screenX = (int)(-minX * 1.2f - i); screenX < maxX + i; screenX++)
                    {
                        mapPosition = cameraPosition + new Vector2(screenX, screenY);

                        if (!Parcel.Instance.IsInParcelBounds(new Vector2(mapPosition.X, mapPosition.Y), currentLevel))
                            continue;

                        screenPosition.X = (screenX - screenY) * Constants.TileWidth / 2;
                        screenPosition.Y = (screenX + screenY) * Constants.TileHeight / 3;

                        screenPosition.X += Constants.HalfScreen.X - Constants.TileWidth / 2;

                        foreach (Dwarf dwarf in Colony.Instance.Members)
                        {
                            if ((int) dwarf.Position.X == (int) mapPosition.X && (int) dwarf.Position.Y == currentLevel - i && (int) dwarf.Position.Z == (int) mapPosition.Y)
                            {
                                Tileset.Instance.DrawTile(spriteBatch, screenPosition + drawOffset * i, 16);

                            }
                        }

                        Tileset.Instance.DrawTile(spriteBatch, screenPosition + drawOffset * i, TileRegistry.Tiles[Parcel.Instance.GetTile(new Vector3(mapPosition.X, currentLevel - i, mapPosition.Y))].TextureID);
                    }
                }
                spriteBatch.End();
            }

        }

        void DrawUI()
        {
            spriteBatch.Begin();
            Vector2 mousePosition = ScreenToMapPosition(Mouse.GetState().Position.ToVector2());

            UIText.Display($"{TileRegistry.Tiles[Parcel.Instance.GetTile(new Vector3(mousePosition.X, currentLevel, mousePosition.Y))].Name}", Color.White, new Vector2(), UIAnchor.DownLeft, true, false);

            UIText.Display($"Colony {Colony.Instance.Name} ({Colony.Instance.Members.Count} members)", Color.White, new Vector2(), UIAnchor.CenterUp); 
            UIText.Display($"{frameRate} fps", Color.Yellow, new Vector2(), UIAnchor.UpRight, true);
            
            if (currentLevel - Constants.SurfaceLevel >= 0)
                UIText.Display($"Altitude: {currentLevel - Constants.SurfaceLevel}", Color.Lime, new Vector2(), UIAnchor.DownRight, true);
            else
                UIText.Display($"Depth: {currentLevel - Constants.SurfaceLevel}", Color.OrangeRed, new Vector2(), UIAnchor.DownRight, true);

            UIText.Display($"Designations:\n - Mine (m)", Color.White, new Vector2(), UIAnchor.CenterLeft, true);

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

            if (gameTime != null)
                frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
