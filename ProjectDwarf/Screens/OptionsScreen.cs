using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDwarf.Managers;
using ProjectDwarf.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Screens
{
    public class OptionsScreen : BaseScreen
    {
        private SpriteBatch spriteBatch;

        public OptionsScreen(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void LoadContent()
        {

        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Escape))
                ScreenManager.Instance.ChangeScreen(new MainMenu(spriteBatch));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            UIText.Display("Options", Color.Yellow, new Vector2(), UIAnchor.CenterUp, false, true);

            spriteBatch.End();
        }
    }
}
