using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDwarf.Managers;
using ProjectDwarf.Screens;
using ProjectDwarf.Utils;
using ProjectDwarf.UI;

namespace ProjectDwarf
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            AssetManager.Instance.Initialize(Content);

            Tileset.Instance.Initialize();

            RNG.Initialize(null);

            Parcel.Instance.WorldGen();

            UnitTesting.Instance.Do();

            if (Constants.Fullscreen)
            {
                Constants.ScreenWidth = 1920;
                Constants.ScreenHeight = 1080;
            }

            _graphics.PreferredBackBufferWidth = Constants.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Constants.ScreenHeight;

            _graphics.IsFullScreen = Constants.Fullscreen;

            Constants.ScreenBounds = new Vector2(Constants.ScreenHeight, Constants.ScreenWidth);

            Constants.HalfScreen = new Vector2(Constants.ScreenWidth / 2, Constants.ScreenHeight / 2);

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Instance.LoadTexture("tileset");

            UIText.Initialize(_spriteBatch);

            ScreenManager.Instance.ChangeScreen(new ColonyScreen(_spriteBatch));
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Instance.CurrentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            ScreenManager.Instance.CurrentScreen.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}