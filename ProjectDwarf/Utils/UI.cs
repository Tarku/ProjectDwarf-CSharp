using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDwarf.Managers;

namespace ProjectDwarf.UI
{
    public enum UIAnchor
    {
        Up,
        UpLeft,   CenterUp,   UpRight,
        CenterLeft, CenterRight,
        Left,     Center,     Right,
        DownLeft, CenterDown, DownRight,
        Down,
        AbsoluteCenter
    }

    public class UIText
    {
        private static SpriteBatch _spriteBatch;
        private static SpriteFont _font;

        private static Texture2D _textBackground;

        public static void Initialize(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

            AssetManager.Instance.LoadFont();
            AssetManager.Instance.LoadTexture("text_background");

            _font = AssetManager.Instance.GetFont();
            _textBackground = AssetManager.Instance.GetTexture("text_background");

            Console.WriteLine("UIText initialized.");
        }

        public static void Display(string text, Color color, Vector2 position, UIAnchor anchor = UIAnchor.Left, bool hasBackground = true)
        {
            Vector2 blitPosition = new();
            Vector2 tSize = _font.MeasureString(text) + Constants.UIPadding ;

            switch (anchor)
            {

                case UIAnchor.Up:
                    blitPosition = new Vector2(position.X, Constants.UIPadding.Y * 2);
                    break;

                case UIAnchor.Left:
                    blitPosition = position + Constants.UIPadding;
                    break;

                case UIAnchor.Right:
                    blitPosition = new Vector2(position.X - tSize.X - Constants.UIPadding.X, position.Y - tSize.Y / 2);
                    break;

                case UIAnchor.CenterLeft:
                    blitPosition = new Vector2(Constants.UIPadding.X * 2, Constants.HalfScreen.Y - tSize.Y / 2);
                    break;

                case UIAnchor.CenterRight:
                    blitPosition = new Vector2(Constants.ScreenWidth - tSize.X - Constants.UIPadding.X, Constants.HalfScreen.Y - tSize.Y / 2);
                    break;

                case UIAnchor.UpLeft:
                    blitPosition = new Vector2(Constants.UIPadding.X * 2, Constants.UIPadding.Y * 2);
                    break;

                case UIAnchor.UpRight:
                    blitPosition = new Vector2(Constants.ScreenWidth - tSize.X - Constants.UIPadding.X, Constants.UIPadding.Y * 2);
                    break;

                case UIAnchor.Down:
                    blitPosition = new Vector2(position.X, Constants.ScreenHeight - Constants.UIPadding.Y - tSize.Y);
                    break;

                case UIAnchor.DownLeft:
                    blitPosition = new Vector2(Constants.UIPadding.X * 2, Constants.ScreenHeight - Constants.UIPadding.Y - tSize.Y);
                    break;

                case UIAnchor.DownRight:
                    blitPosition = new Vector2(Constants.ScreenWidth - Constants.UIPadding.X - tSize.X, Constants.ScreenHeight - Constants.UIPadding.Y - tSize.Y);
                    break;

                case UIAnchor.CenterDown:
                    blitPosition = new Vector2(Constants.HalfScreen.X - tSize.X / 2, Constants.ScreenHeight - Constants.UIPadding.Y - tSize.Y);
                    break;

                case UIAnchor.CenterUp:
                    blitPosition = new Vector2(Constants.HalfScreen.X - tSize.X / 2, Constants.UIPadding.Y * 2);
                    break;

                case UIAnchor.Center:
                    blitPosition = new Vector2(Constants.HalfScreen.X - tSize.X / 2, position.Y);
                    break;

                case UIAnchor.AbsoluteCenter:
                    blitPosition = Constants.HalfScreen - tSize / 2;
                    break;
            }

            Rectangle tRectangle = new(blitPosition.ToPoint() - Constants.UIPadding.ToPoint(), tSize.ToPoint() + Constants.UIPadding.ToPoint());

            if (hasBackground)
                _spriteBatch.Draw(_textBackground, tRectangle, Color.White);

            _spriteBatch.DrawString(_font, text, blitPosition, color);
        }
    }
}
