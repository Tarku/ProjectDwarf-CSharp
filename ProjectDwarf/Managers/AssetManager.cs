using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Managers
{
    public class AssetManager
    {
        private static AssetManager instance;
        private static Dictionary<string, Texture2D> textures;

        private static SpriteFont font;
        private static SpriteFont titleFont;

        private ContentManager contentManager;

        public AssetManager()
        {
            textures = new Dictionary<string, Texture2D>();
        }

        public static AssetManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AssetManager();

                return instance;
            }
        }

        public void Initialize(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public void LoadTexture(string textureName)
        {
            textures[textureName] = contentManager.Load<Texture2D>(textureName);
        }

        public void LoadFonts()
        {
            font = contentManager.Load<SpriteFont>("font");
            titleFont = contentManager.Load<SpriteFont>("titlefont");
        }

        public void LoadTextures(string[] textureNames)
        {
            foreach (string textureName in textureNames)
                textures[textureName] = contentManager.Load<Texture2D>(textureName);
        }

        public SpriteFont GetRegularFont()
        {
            return font;
        }

        public SpriteFont GetTitleFont()
        {
            return titleFont;
        }

        public Texture2D GetTexture(string textureName)
        {
            return textures.GetValueOrDefault(textureName);
        }

    }
}
