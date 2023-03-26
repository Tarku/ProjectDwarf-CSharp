using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDwarf.UI;
using Microsoft.Xna.Framework.Input;
using ProjectDwarf.Managers;

namespace ProjectDwarf.Screens
{
    public class MainMenu : BaseScreen
    {
        private SpriteBatch spriteBatch;
        private int menuChoiceIndex = 0;
        private float arrowKeyTimer = 0f;

        private string[] menuChoiceStrings = new string[] { "Play game", "Options", "Quit" };

        public MainMenu(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void LoadContent()
        {

        }

        public override void UnloadContent()
        {

        }

        void HandleSelection()
        {
            switch (menuChoiceIndex)
            {
                case 0:
                    ScreenManager.Instance.ChangeScreen(Game1.ColonyScreen) ;
                    break;
                case 1:
                    ScreenManager.Instance.ChangeScreen(Game1.OptionsScreen);
                    break;
                case 2:
                    Game1.shouldQuit = true;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Down) && arrowKeyTimer > .5f)
            {
                menuChoiceIndex++;
                arrowKeyTimer = 0f;
            }
            if (kb.IsKeyDown(Keys.Up) && arrowKeyTimer > .5f)
            {
                menuChoiceIndex--;
                arrowKeyTimer = 0f;
            }


            if (menuChoiceIndex == menuChoiceStrings.Length)
                menuChoiceIndex = 0;

            if (menuChoiceIndex < 0)
                menuChoiceIndex = menuChoiceStrings.Length - 1;

            arrowKeyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kb.IsKeyDown(Keys.Enter))
                HandleSelection();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            UIText.Display("Project Dwarf", Color.Yellow, new Vector2(), UIAnchor.CenterUp, false, true);

            for (int i = 0; i < menuChoiceStrings.Length; i++)
            {
                Color color = Color.Gray;
                string menuChoiceString = menuChoiceStrings[i];

                if (menuChoiceIndex == i)
                {
                    color = Color.White;
                    menuChoiceString = String.Concat(">", menuChoiceString, " <");
                }

                UIText.Display(menuChoiceString, color, new Vector2(Constants.ScreenHeight / 4 + i * 35), UIAnchor.Center, false, false);
            }
            UIText.Display(Constants.Version, Color.White, new Vector2(), UIAnchor.DownLeft, false, false);

            spriteBatch.End();
        }
    }
}
