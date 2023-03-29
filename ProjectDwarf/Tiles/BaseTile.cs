using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Tiles
{
    public class BaseTile
    {
        public string Name;
        public byte TextureID;
        public bool IsPassable;

        public BaseTile(string name, byte textureId, bool isPassable = false)
        {
            Name = name;
            TextureID = textureId;

            IsPassable = isPassable;
        }
    }
}
