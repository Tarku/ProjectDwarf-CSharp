using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDwarf.UI;

namespace ProjectDwarf.Screens
{
    public class TestScreen : BaseScreen
    {
        int i = 0;

        public override void LoadContent()
        {

        }

        public override void UnloadContent()
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            UIText.Display($"A{i}", Color.Red, new Vector2(), UIAnchor.UpLeft);
            UIText.Display($"B{i}", Color.Orange, new Vector2(), UIAnchor.CenterUp);
            UIText.Display($"C{i}", Color.Yellow, new Vector2(), UIAnchor.UpRight);

            UIText.Display($"D{i}", Color.Green, new Vector2(), UIAnchor.CenterLeft);
            UIText.Display($"E{i}", Color.Cyan, new Vector2(), UIAnchor.AbsoluteCenter);
            UIText.Display($"F{i}", Color.Blue, new Vector2(), UIAnchor.CenterRight);

            UIText.Display($"G{i}", Color.Magenta, new Vector2(), UIAnchor.DownLeft);
            UIText.Display($"H{i}", Color.Red, new Vector2(), UIAnchor.CenterDown);
            UIText.Display($"I{i}", Color.Orange, new Vector2(), UIAnchor.DownRight);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            i++;
        }
    }
}
